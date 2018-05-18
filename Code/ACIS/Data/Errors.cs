namespace Data
{
   public enum Errors
    {
        Success = 0,
        ReceivedFewerThanTwoBytes = -1,
        InvalidOperation = -2,
        InvalidDevice = -3,
        CouldNotPerformOperation = -4,
        CouldNotDecodeBytes = -5,
        CouldNotProcessBuffer = -6,
        StopInterrupt = -7
    }
}
