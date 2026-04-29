namespace backend.Exceptions
{
    /// <summary>
    /// Exception levée quand une ressource n'est pas trouvée.
    /// Correspond à une réponse HTTP 404 Not Found.
    /// </summary>
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception levée quand la validation des données échoue.
    /// Correspond à une réponse HTTP 400 Bad Request.
    /// </summary>
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception levée quand l'utilisateur n'est pas authentifié.
    /// Correspond à une réponse HTTP 401 Unauthorized.
    /// </summary>
    [Serializable]
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() { }
        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception levée quand l'utilisateur est authentifié mais n'a pas les permissions requises.
    /// Correspond à une réponse HTTP 403 Forbidden.
    /// </summary>
    [Serializable]
    public class ForbiddenException : Exception
    {
        public ForbiddenException() { }
        public ForbiddenException(string message) : base(message) { }
        public ForbiddenException(string message, Exception inner) : base(message, inner) { }
    }
}
