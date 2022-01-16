namespace Dataverse.Habr.Intro;

public interface IHandler
{
    string Text { get; }
    string Handle(ConnectionProvider connectionProvider);
}