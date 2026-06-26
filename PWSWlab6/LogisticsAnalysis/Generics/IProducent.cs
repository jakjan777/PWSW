namespace LogisticsAnalysis.Generics;

public interface IProducent<out T>
{
    T Utworz();
}
