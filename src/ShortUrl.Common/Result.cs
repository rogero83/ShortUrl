namespace ShortUrl.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace ResultPattern
    {
        /// <summary>
        /// Rappresenta il risultato di un'operazione senza valore di ritorno
        /// </summary>
        public class Result
        {
            public bool IsSuccess { get; }
            public bool IsFailure => !IsSuccess;
            public Error Error { get; }
            public List<Error> Errors { get; }

            protected Result(bool isSuccess, Error error)
            {
                if (isSuccess && error != Error.None)
                    throw new InvalidOperationException("A successful result cannot contain an error");

                if (!isSuccess && error == Error.None)
                    throw new InvalidOperationException("A failure result must contain an error");

                IsSuccess = isSuccess;
                Error = error;
                Errors = [];
                if (error != Error.None)
                    Errors.Add(error);
            }

            protected Result(bool isSuccess, IEnumerable<Error> errors)
            {
                if (isSuccess && errors.Any())
                    throw new InvalidOperationException("A successful result cannot contain an error");

                if (!isSuccess && !errors.Any())
                    throw new InvalidOperationException("A failure result must contain an error");

                IsSuccess = isSuccess;
                Errors = [.. errors];
                Error = Errors.FirstOrDefault() ?? Error.None;
            }

            // Factory methods
            public static Result Success() => new(true, Error.None);
            public static Result Failure(Error error) => new(false, error);
            public static Result Failure(IEnumerable<Error> errors) => new(false, errors);
            public static Result Failure(string message) => new(false, new Error(message));

            // Implicit conversion da Result<T> a Result
            public static implicit operator Result(Error error) => Failure(error);

            // Combina più risultati
            public static Result Combine(params Result[] results)
            {
                var failures = results.Where(r => r.IsFailure).ToList();

                if (!failures.Any())
                    return Success();

                var allErrors = failures.SelectMany(f => f.Errors);
                return Failure(allErrors);
            }

            // Pattern matching helper
            public T Match<T>(Func<T> onSuccess, Func<Error, T> onFailure)
            {
                return IsSuccess ? onSuccess() : onFailure(Error);
            }

            public void Match(Action onSuccess, Action<Error> onFailure)
            {
                if (IsSuccess)
                    onSuccess();
                else
                    onFailure(Error);
            }
        }

        /// <summary>
        /// Rappresenta il risultato di un'operazione con valore di ritorno
        /// </summary>
        public class Result<T> : Result
        {
            private readonly T _value;

            public T Value
            {
                get
                {
                    if (IsFailure)
                        throw new InvalidOperationException("Non è possibile accedere al valore di un risultato fallito");

                    return _value;
                }
            }

            protected internal Result(T value, bool isSuccess, Error error)
                : base(isSuccess, error)
            {
                _value = value;
            }

            protected internal Result(T value, bool isSuccess, IEnumerable<Error> errors)
                : base(isSuccess, errors)
            {
                _value = value;
            }

            // Factory methods
            public static Result<T> Success(T value) => new Result<T>(value, true, Error.None);
            public static new Result<T> Failure(Error error) => new Result<T>(default!, false, error);
            public static new Result<T> Failure(IEnumerable<Error> errors) => new Result<T>(default!, false, errors);
            public static new Result<T> Failure(string message) => new Result<T>(default!, false, new Error(message));

            // Implicit conversions
            public static implicit operator Result<T>(T value) => Success(value);
            public static implicit operator Result<T>(Error error) => Failure(error);

            // Pattern matching con valore
            public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
            {
                return IsSuccess ? onSuccess(Value) : onFailure(Error);
            }

            public void Match(Action<T> onSuccess, Action<Error> onFailure)
            {
                if (IsSuccess)
                    onSuccess(Value);
                else
                    onFailure(Error);
            }

            // Map: trasforma il valore se il risultato è di successo
            public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
            {
                return IsSuccess
                    ? Result<TNew>.Success(mapper(Value))
                    : Result<TNew>.Failure(Error);
            }

            // Bind: concatena operazioni che restituiscono Result
            public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
            {
                return IsSuccess
                    ? binder(Value)
                    : Result<TNew>.Failure(Error);
            }

            // Tap: esegue un'azione senza modificare il risultato
            public Result<T> Tap(Action<T> action)
            {
                if (IsSuccess)
                    action(Value);

                return this;
            }

            // TapError: esegue un'azione sull'errore senza modificare il risultato
            public Result<T> TapError(Action<Error> action)
            {
                if (IsFailure)
                    action(Error);

                return this;
            }

            // Ensure: verifica una condizione
            public Result<T> Ensure(Func<T, bool> predicate, Error error)
            {
                if (IsFailure)
                    return this;

                return predicate(Value)
                    ? this
                    : Result<T>.Failure(error);
            }

            // GetValueOrDefault
            public T GetValueOrDefault(T defaultValue = default!)
            {
                return IsSuccess ? Value : defaultValue;
            }

            // GetValueOrThrow
            public T GetValueOrThrow()
            {
                if (IsFailure)
                    throw new InvalidOperationException($"Operation failed: {Error.Message}");

                return Value;
            }
        }

        /// <summary>
        /// Rappresenta un errore
        /// </summary>
        public sealed class Error
        {
            public string Message { get; }
            public ErrorType Type { get; }
            public Dictionary<string, object> Metadata { get; }

            public Error(string message, ErrorType type = ErrorType.Failure)
            {
                Message = message;
                Type = type;
                Metadata = new Dictionary<string, object>();
            }

            public static readonly Error None = new Error(string.Empty, ErrorType.None);

            // Metodi di creazione tipizzati
            public static Error Validation(string message) =>
                new Error(message, ErrorType.Validation);

            public static Error NotFound(string message) =>
                new Error(message, ErrorType.NotFound);

            public static Error Conflict(string message) =>
                new Error(message, ErrorType.Conflict);

            public static Error Unauthorized(string message) =>
                new Error(message, ErrorType.Unauthorized);

            public static Error Forbidden(string message) =>
                new Error(message, ErrorType.Forbidden);

            public static Error Failure(string message) =>
                new Error(message, ErrorType.Failure);

            // Aggiungi metadata
            public Error WithMetadata(string key, object value)
            {
                var error = new Error(Message, Type);
                foreach (var kvp in Metadata)
                    error.Metadata[kvp.Key] = kvp.Value;
                error.Metadata[key] = value;
                return error;
            }

            public override int GetHashCode() => HashCode.Combine(Message, Type);

            public override string ToString() => $"[{Type}] : {Message}";
        }

        /// <summary>
        /// Tipi di errore
        /// </summary>
        public enum ErrorType
        {
            None = 0,
            Failure = 1,
            Validation = 2,
            NotFound = 3,
            Conflict = 4,
            Unauthorized = 5,
            Forbidden = 6
        }
    }
}
