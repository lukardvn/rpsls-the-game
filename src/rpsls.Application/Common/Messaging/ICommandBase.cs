using MediatR;

namespace rpsls.Application.Common.Messaging;

//TODO: do I need?
public interface ICommandBase
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>//, ICommandBase
{
}

public interface ICommand : IRequest//, ICommandBase
{
}