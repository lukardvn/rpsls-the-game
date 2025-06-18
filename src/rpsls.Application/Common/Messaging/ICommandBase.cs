using MediatR;

namespace rpsls.Application.Common.Messaging;

public interface ICommandBase;

public interface ICommand<out TResponse> : IRequest<TResponse>, ICommandBase;

public interface ICommand : IRequest, ICommandBase;