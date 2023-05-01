namespace API_TestProject.Core
{
    public static class TestsService
    {
        public static bool IsTestsRunning { get; set; } = false;
        public static bool IsDataBaseReinitialized { get; set; } = false;

        public static bool ShouldDataBaseBeReinitialized => IsTestsRunning && !IsDataBaseReinitialized;
    }
}
