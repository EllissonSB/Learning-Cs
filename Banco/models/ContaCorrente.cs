namespace models;
public class ContaCorrente : ContaBancaria, ITributavel
{
    public decimal LimiteChequeEspecial { get; set; } = 500m; // Limite padrão de R$500
    public decimal TaxaManutencaoMensal { get; set; } = 10m; // Taxa de manutenção mensal padrão de R$10
    public ContaCorrente() : base() {}

    public ContaCorrente(User titular) : base(titular) {}

    public override void Sacar(decimal valor, string descricao)
    {
        if (valor > Saldo + LimiteChequeEspecial)
        {
            Console.WriteLine("Operação inválida: Excede o limite do cheque especial.");
            return;
        }
        base.ExecutarSaque(valor, descricao);
    }
    public decimal CalcularImposto()
    {
        // Exemplo: Imposto de 1% sobre o saldo atual
        return Saldo * 0.01m;
    }
}