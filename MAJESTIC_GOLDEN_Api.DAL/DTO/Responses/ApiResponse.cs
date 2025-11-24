namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message_En { get; set; } = string.Empty;
        public string Message_Ar { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string messageEn, string messageAr)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message_En = messageEn,
                Message_Ar = messageAr,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string messageEn, string messageAr, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message_En = messageEn,
                Message_Ar = messageAr,
                Errors = errors
            };
        }
    }
}



