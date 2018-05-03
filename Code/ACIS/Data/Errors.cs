namespace Data
{
   public enum Errors
    {
        SUCCESS = 0,
        RECEIVED_FEWER_THAN_TWO_BYTES = -1,
        INVALID_OPERATION = -2,
        INVALID_DEVICE = -3,
        COULD_NOT_PERFORM_OPERATION = -4,
        COULD_NOT_DECODE_BYTES = -5,
        COULD_NOT_PROCESS_BUFFER = -6,
        STOP_INTERRUPT = -7
    }
}
