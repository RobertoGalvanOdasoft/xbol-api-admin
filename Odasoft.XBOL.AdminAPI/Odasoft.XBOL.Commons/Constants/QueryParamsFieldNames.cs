namespace Odasoft.XBOL.Commons.Constants
{
    /// <summary>
    /// Provides constant field names for suite-related query parameters.
    /// </summary>
    /// <remarks>Use these constants to avoid hardcoding query parameter names when constructing requests that
    /// filter or identify test suites by name or level.
    /// Also take in consideration that the values must be lower case.</remarks>
    public static class QueryParamsFieldNames
    {
        public const string SUITE_NAME = "name";
        public const string SUITE_LEVEL = "level";
    }
}
