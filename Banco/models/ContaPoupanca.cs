namespace models;
public class ContaPoupanca : ContaBancaria
{
    public decimal TaxaJurosAnual { get; set; } = 0.05m; // 3% ao ano por padrão

    public ContaPoupanca() : base() {}

    public ContaPoupanca(User titular) : base(titular) {}

    public override void Sacar(decimal valor, string descricao)
    {
        if (valor > Saldo)
        {
            Console.WriteLine("Operação inválida: Saldo insuficiente para saque em Conta Poupança.");
            return;
        }
        base.ExecutarSaque(valor, descricao);
    }

    public void AplicarJurosMensais()
    {
        decimal jurosMensais = Saldo * (TaxaJurosAnual / 12);
        Depositar(jurosMensais, "Juros Mensais");
        Console.Write($"Juros mensais de {jurosMensais:C} aplicados à Conta Poupança de {Titular.Nome}.");
    }
}