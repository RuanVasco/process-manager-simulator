namespace ProcessManagerSimulator.Models {
    public class Process(int arrivalTime, int executionTime) {
        private static int _nextPid = 1;

        public int Pid { get; private set; } = _nextPid++;
        public int ArrivalTime { get; set; } = arrivalTime;
        public int ExecutionTime { get; set; } = executionTime;
        public int RemainingTime { get; set; } = executionTime;
    }
}
