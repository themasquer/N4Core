namespace N4Core.Responses.Managers
{
    public class ResponseManager
    {
        public virtual ErrorResponse Error(string message) => new ErrorResponse(message);
        public virtual ErrorResponse Error() => new ErrorResponse();
        public virtual ErrorResponse<TResponseType> Error<TResponseType>(string message, TResponseType data) => new ErrorResponse<TResponseType>(message, data);
        public virtual ErrorResponse<TResponseType> Error<TResponseType>(string message) => new ErrorResponse<TResponseType>(message);
        public virtual ErrorResponse<TResponseType> Error<TResponseType>(TResponseType data) => new ErrorResponse<TResponseType>(data);
        public virtual ErrorResponse<TResponseType> Error<TResponseType>() => new ErrorResponse<TResponseType>();
        public virtual SuccessResponse Success(string message, int id) => new SuccessResponse(message, id);
        public virtual SuccessResponse Success(string message) => new SuccessResponse(message);
        public virtual SuccessResponse Success() => new SuccessResponse();
        public virtual SuccessResponse<TResponseType> Success<TResponseType>(string message, TResponseType data) => new SuccessResponse<TResponseType>(message, data);
        public virtual SuccessResponse<TResponseType> Success<TResponseType>(string message) => new SuccessResponse<TResponseType>(message);
        public virtual SuccessResponse<TResponseType> Success<TResponseType>(TResponseType data) => new SuccessResponse<TResponseType>(data);
        public virtual SuccessResponse<TResponseType> Success<TResponseType>() => new SuccessResponse<TResponseType>();
    }
}
