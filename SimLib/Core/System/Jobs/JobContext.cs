namespace SimLib.Core.System.Jobs;

public readonly struct JobContext(int threadIndex)
{
    public readonly int ThreadIndex = threadIndex;
}