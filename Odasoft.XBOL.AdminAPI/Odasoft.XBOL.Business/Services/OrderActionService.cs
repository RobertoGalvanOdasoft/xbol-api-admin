using Hangfire;
using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Constants;
using Odasoft.XBOL.Commons.Email;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Commons.Requests;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Data.Repositories.Order;
using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Responses;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class OrderActionService
    {
        private readonly OrderActionLogRepository _orderActionLogRepository;
        private readonly SeasonPassRepository _seasonPassRepository;
        private readonly TicketRepository _ticketRepository;
        private readonly OrderRepository _orderRepository;

        private readonly OrderService _orderService;

        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ITicketingClient _ticketetingClient;

        private const string DEFAULT_CULTURE = "es-MX";

        public OrderActionService(
            OrderActionLogRepository orderActionLogRepository,
            SeasonPassRepository seasonPassRepository,
            TicketRepository tickeyRepository,
            OrderRepository orderRepository,
            ITicketingClient ticketingClient,
            OrderService orderService,
            IBackgroundJobClient backgroundJobClient)
        {
            _orderActionLogRepository = orderActionLogRepository;
            _seasonPassRepository = seasonPassRepository;
            _ticketRepository = tickeyRepository;
            _orderRepository = orderRepository;
            _ticketetingClient = ticketingClient;
            _orderService = orderService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<bool?> PerformOrderActionAsync(long orderId, OrderActionRequest request)
        {
            try
            {
                bool actionResult = request.Action switch
                {
                    OrderAction.OrderCreated => true,
                    OrderAction.OrderRenewed => true,
                    OrderAction.CancelOrder => await CancelOrderAsync(orderId, request),
                    OrderAction.CancelWithoutRefund => await CancelOrderAsync(orderId, request),
                    OrderAction.ResendReceipt => await EmailResendReceiptAsync(orderId, request),
                    OrderAction.UpdateOrderHolder => true,
                    OrderAction.ReissueTickets => await EmailReIssueTicketsAsync(orderId, request),
                    OrderAction.SendIndiviualTickets => await EmailSendTicketsAsync(orderId, request),
                    OrderAction.ResendCourtesyTickets => await EmailSendCourtesiesAsync(orderId, request),
                    OrderAction.ConvertToDigitalPyshical => await ConvertToDigitalPhysicalAsync(orderId, request),
                    OrderAction.CancelTickets => await CancelItemsAsync(orderId, request),
                    _ => false
                };

                if (actionResult == false)
                {
                    Console.WriteLine($"Action {request.Action} failed for order {orderId}.");
                    return false;
                }

                await SaveLogAsync(orderId, request);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error performing order action: " + ex.Message);
                return null;
            }
        }

        public async Task<List<OrderActionResponse>> GetOrderLogsAsync(long orderId)
        {
            return await _orderActionLogRepository
                            .Get()
                            .AsNoTracking()
                            .Where(x => x.OrderId == orderId)
                            .Select(x => new OrderActionResponse
                            {
                                OrderId = x.OrderId,
                                Seats = x.Seats,
                                Action = x.Action,
                                ActionName = x.ActionName,
                                Comments = x.Comments,
                                CreatedAt = x.CreatedAt
                            }).ToListAsync();
        }

        // This will handle both cancel with refund and without refund scenarios based on the request details
        private async Task<bool> CancelOrderAsync(long orderId, OrderActionRequest request)
        {
            try
            {
                Order? order = await _orderRepository
                                        .Get()
                                        .Where(o => o.Id == orderId)
                                        .SingleOrDefaultAsync();

                if (order == null)
                {
                    return false;
                }

                ReleaseBookedSeatsRequest? releaseRequest = await GetEventKeyAndSeatsAsync(order);

                if (releaseRequest == null)
                {
                    return false;
                }

                if (!await ReleaseBookingAsync(releaseRequest))
                {
                    return false;
                }

                if (order.OrderType == OrderType.SeasonPass)
                {
                    var seasonPasses = await _seasonPassRepository
                                            .Get()
                                            .Where(sp => order.Items.Select(i => i.ItemReferenceId).Contains(sp.Id))
                                            .ToListAsync();

                    foreach (var sp in seasonPasses)
                    {
                        sp.Status = SeasonPassStatus.Cancelled;
                    }

                    await _seasonPassRepository.CommitAsync();
                }
                else
                {
                    var tickets = await _ticketRepository
                                            .Get()
                                            .Where(t => order.Items.Select(i => i.ItemReferenceId).Contains(t.Id))
                                            .ToListAsync();

                    foreach (var ticket in tickets)
                    {
                        ticket.Status = TicketStatus.Cancelled;
                    }

                    await _ticketRepository.CommitAsync();
                }

                order.Status = OrderStatus.Cancelled;

                await _orderRepository.CommitAsync();
                await EmailCancelOrderAsync(orderId, request);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error performing cancel order action: " + ex.Message);
                return false;
            }
        }

        private async Task<bool> ReleaseBookingAsync(ReleaseBookedSeatsRequest releaseRequest)
        {
            try
            {
                ChangeObjectStatusResult cosr = await _ticketetingClient.ReleaseBookedSeatsAsync(releaseRequest);
                // TODO: We might want to check the result here to ensure the release was fully successful before returning true.

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not release seats for event '{releaseRequest.Key}'. Error: {ex.Message}");
                return false;
            }
        }

        private async Task<ReleaseBookedSeatsRequest?> GetEventKeyAndSeatsAsync(Order order)
        {
            List<long> seatIds = order.Items.Select(i => i.ItemReferenceId).ToList();

            if (seatIds.Count == 0)
            {
                return null;
            }

            if (order.OrderType == OrderType.SeasonPass)
            {
                return await GetSeasonReleaseBookingRequest(seatIds);
            }

            return await GetEventReleaseBookingRequest(seatIds);
        }

        private async Task SaveLogAsync(long orderId, OrderActionRequest request)
        {
            await _orderActionLogRepository.InsertAsync(new OrderActionLog
            {
                OrderId = orderId,
                Action = request.Action,
                ActionName = request.ActionName,
                Comments = request.Comments,
                Seats = string.Join(",", request.Seats.Values),
                CreatedAt = DateTimeOffset.Now.ToUniversalTime(),
                CreatedBy = Guid.NewGuid() // This should ideally come from the authenticated user context
            });

            await _orderActionLogRepository.CommitAsync();
        }

        private async Task<ReleaseBookedSeatsRequest?> GetEventReleaseBookingRequest(List<long> seatIds)
        {
            List<string> seatKeys = [];
            string key = "";

            var tickets = await _ticketRepository
                                        .Get()
                                        .AsNoTracking()
                                        .Where(t => seatIds.Contains(t.Id)
                                            && t.Status != TicketStatus.Cancelled)
                                        .ToListAsync();

            return new ReleaseBookedSeatsRequest
            {
                Key = key,
                Seats = seatKeys
            };
        }

        private async Task<ReleaseBookedSeatsRequest?> GetSeasonReleaseBookingRequest(List<long> seatIds)
        {
            List<string> seatKeys = [];
            string key = "";

            var seasonPasses = await _seasonPassRepository
                                        .Get()
                                        .AsNoTracking()
                                        .Where(sp => seatIds.Contains(sp.Id)
                                            && sp.Status != SeasonPassStatus.Cancelled)
                                        .ToListAsync();

            if (seasonPasses.Count == 0)
            {
                return null;
            }

            key = seasonPasses.First().Season.ExternalSeasonKey;

            seatKeys.AddRange(seasonPasses.Select(sp => sp.TrackingCode));

            return new ReleaseBookedSeatsRequest
            {
                Key = key,
                Seats = seatKeys
            };
        }

        private async Task<bool> CancelItemsAsync(long orderId, OrderActionRequest request)
        {
            try
            {
                List<long> seatIds = request.Seats.Keys.ToList();

                if (seatIds.Count == 0)
                {
                    return false;
                }

                Order? order = await _orderRepository
                                        .Get()
                                        .AsNoTracking()
                                        .Where(o => o.Id == orderId)
                                        .SingleOrDefaultAsync();

                if (order == null)
                {
                    return false;
                }

                if (order.OrderType == OrderType.SeasonPass)
                {
                    ReleaseBookedSeatsRequest? releaseRequest = await GetSeasonReleaseBookingRequest(seatIds);

                    if (releaseRequest == null)
                    {
                        return false;
                    }

                    if (!await ReleaseBookingAsync(releaseRequest))
                    {
                        return false;
                    }

                    await CancelSeasonPassesAsync(seatIds);
                }
                else
                {
                    ReleaseBookedSeatsRequest? releaseRequest = await GetEventReleaseBookingRequest(seatIds);

                    if (releaseRequest == null)
                    {
                        return false;
                    }

                    if (!await ReleaseBookingAsync(releaseRequest))
                    {
                        return false;
                    }

                    await CancelTicketsAsync(seatIds);
                }

                await EmailCancelOrderTicketAsync(orderId, request);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to cancel tickets for order '{orderId}'. Error; {ex.Message}");
                return false;
            }
        }

        private async Task CancelTicketsAsync(List<long> ticketIds)
        {
            var tickets = await _ticketRepository
                                .Get()
                                .Where(t => ticketIds.Contains(t.Id)
                                    && t.Status != TicketStatus.Cancelled)
                                .ToListAsync();

            foreach (var ticket in tickets)
            {
                ticket.Status = TicketStatus.Cancelled;
            }

            await _ticketRepository.CommitAsync();
        }

        private async Task CancelSeasonPassesAsync(List<long> seasonPassIds)
        {
            var seasonPasses = await _seasonPassRepository
                                        .Get()
                                        .Where(sp => seasonPassIds.Contains(sp.Id)
                                            && sp.Status != SeasonPassStatus.Cancelled)
                                        .ToListAsync();

            foreach (var sp in seasonPasses)
            {
                sp.Status = SeasonPassStatus.Cancelled;
            }

            await _seasonPassRepository.CommitAsync();
        }

        private async Task<bool> ConvertToDigitalPhysicalAsync(long orderId, OrderActionRequest request)
        {
            try
            {
                List<long> seatIds = request.Seats.Keys.ToList();

                if (seatIds.Count == 0)
                {
                    return false;
                }

                Order? order = await _orderRepository
                                        .Get()
                                        .AsNoTracking()
                                        .Where(o => o.Id == orderId)
                                        .SingleOrDefaultAsync();

                if (order == null)
                {
                    return false;
                }

                if (order.OrderType == OrderType.SeasonPass)
                {
                    List<SeasonPass> seasonPasses = await _seasonPassRepository
                                            .Get()
                                            .Where(sp => seatIds.Contains(sp.Id)
                                                && sp.Status != SeasonPassStatus.Cancelled
                                                && sp.IsDigital == !request.ChangeToDigital)
                                            .ToListAsync();

                    foreach (var sp in seasonPasses)
                    {
                        sp.IsDigital = request.ChangeToDigital;
                    }

                    await _seasonPassRepository.CommitAsync();
                }
                else
                {
                    List<Ticket> tickets = await _ticketRepository
                                            .Get()
                                            .Where(t => seatIds.Contains(t.Id)
                                                && t.Status != TicketStatus.Cancelled
                                                && t.IsDigital == !request.ChangeToDigital)
                                            .ToListAsync();

                    foreach (var ticket in tickets)
                    {
                        ticket.IsDigital = request.ChangeToDigital;
                    }

                    await _ticketRepository.CommitAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to convert tickets for order '{orderId}'. Error; {ex.Message}");
                return false;
            }
        }

        private async Task<bool> EmailResendReceiptAsync(long orderId, OrderActionRequest request)
        {
            try
            {
                Order? order = await _orderRepository
                                        .Get()
                                        .Include(o => o.Client)
                                        .AsNoTracking()
                                        .Where(o => o.Id == orderId)
                                        .SingleOrDefaultAsync();

                if (order == null)
                {
                    return false;
                }

                var model = await _orderService.BuildOrderEmailModelAsync(orderId, request.NewEmail, "", DEFAULT_CULTURE);

                model.Subject = request.ActionName;

                _backgroundJobClient.Enqueue<IEmailJob>(x => x.SendOrderEmailAsync(model, EmailTemplateConstants.ORDER_CONFIRMATION));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to resend order's confirmation for order '{orderId}'. Error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> EmailReIssueTicketsAsync(long orderId, OrderActionRequest request)
        {
            try
            {
                Order? order = await _orderRepository
                                        .Get()
                                        .Include(o => o.Client)
                                        .AsNoTracking()
                                        .Where(o => o.Id == orderId)
                                        .SingleOrDefaultAsync();

                if (order == null)
                {
                    return false;
                }

                OrderEmailModel model = await _orderService.BuildOrderEmailModelAsync(orderId, order.Client?.Email, order.Client?.FullName, DEFAULT_CULTURE);
                model.Subject = request.ActionName;
                model.Seats.RemoveAll(s => !request.Seats.Values.Contains(s.SeatKey));

                _backgroundJobClient.Enqueue<IEmailJob>(x => x.SendOrderEmailAsync(model, EmailTemplateConstants.ORDER_REIISUE_TICKETS));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to reissue order's tickets for order '{orderId}'. Error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> EmailSendTicketsAsync(long orderId, OrderActionRequest request)
        {
            try
            {
                Order? order = await _orderRepository
                                        .Get()
                                        .Include(o => o.Client)
                                        .AsNoTracking()
                                        .Where(o => o.Id == orderId)
                                        .SingleOrDefaultAsync();

                if (order == null)
                {
                    return false;
                }

                OrderEmailModel model = await _orderService.BuildOrderEmailModelAsync(orderId, request.NewEmail, "", DEFAULT_CULTURE);
                model.Subject = request.ActionName;
                model.Seats.RemoveAll(s => !request.Seats.Values.Contains(s.SeatKey));

                _backgroundJobClient.Enqueue<IEmailJob>(x => x.SendOrderEmailAsync(model, EmailTemplateConstants.ORDER_SEND_TICKETS));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to send order's tickets for order '{orderId}'. Error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> EmailSendCourtesiesAsync(long orderId, OrderActionRequest request)
        {
            try
            {
                Order? order = await _orderRepository
                                        .Get()
                                        .Include(o => o.Client)
                                        .AsNoTracking()
                                        .Where(o => o.Id == orderId)
                                        .SingleOrDefaultAsync();

                if (order == null)
                {
                    return false;
                }

                OrderEmailModel model = await _orderService.BuildOrderEmailModelAsync(orderId, request.NewEmail, "", DEFAULT_CULTURE);
                model.Subject = request.ActionName;
                model.Seats.RemoveAll(s => !request.Seats.Values.Contains(s.SeatKey));

                _backgroundJobClient.Enqueue<IEmailJob>(x => x.SendOrderEmailAsync(model, EmailTemplateConstants.ORDER_CANCELLED));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to send order's courtesies for order '{orderId}'. Error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> EmailCancelOrderAsync(long orderId, OrderActionRequest request)
        {
            try
            {
                Order? order = await _orderRepository
                                        .Get()
                                        .Include(o => o.Client)
                                        .AsNoTracking()
                                        .Where(o => o.Id == orderId)
                                        .SingleOrDefaultAsync();

                if (order == null)
                {
                    return false;
                }

                OrderEmailModel model = await _orderService.BuildOrderEmailModelAsync(orderId, request.NewEmail, "", DEFAULT_CULTURE);
                model.Subject = request.ActionName;

                _backgroundJobClient.Enqueue<IEmailJob>(x => x.SendOrderEmailAsync(model, EmailTemplateConstants.ORDER_CANCELLED));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to send order's courtesies for order '{orderId}'. Error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> EmailCancelOrderTicketAsync(long orderId, OrderActionRequest request)
        {
            try
            {
                Order? order = await _orderRepository
                                        .Get()
                                        .Include(o => o.Client)
                                        .AsNoTracking()
                                        .Where(o => o.Id == orderId)
                                        .SingleOrDefaultAsync();

                if (order == null)
                {
                    return false;
                }

                OrderEmailModel model = await _orderService.BuildOrderEmailModelAsync(orderId, request.NewEmail, "", DEFAULT_CULTURE);
                model.Subject = request.ActionName;
                model.Seats.RemoveAll(s => !request.Seats.Values.Contains(s.SeatKey));

                _backgroundJobClient.Enqueue<IEmailJob>(x => x.SendOrderEmailAsync(model, EmailTemplateConstants.ORDER_CANCELLED));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to send order's courtesies for order '{orderId}'. Error: {ex.Message}");
                return false;
            }
        }
    }
}
