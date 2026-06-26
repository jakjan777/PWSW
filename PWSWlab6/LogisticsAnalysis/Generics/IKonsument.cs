namespace LogisticsAnalysis.Generics;

public interface IKonsument<in T>
{
    void Przetworz(T element);
}
