namespace com.gamecodeplus.arkmetricsexporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var processor = new ArkMetricsProcessor();
            processor.Run();
        }
    }
}
