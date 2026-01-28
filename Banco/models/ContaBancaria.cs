namespace models;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

// AVISO AO JSON: "Existem classes filhas! Se o tipo for 'poupanca', use a classe ContaPoupanca"
[JsonDerivedType(typeof(ContaBancaria), typeDiscriminator: "padrao")]
[JsonDerivedType(typeof(ContaPoupanca), typeDiscriminator: "poupanca")]
[JsonDerivedType(typeof(ContaCorrente), typeDiscriminator: "corrente")]


public class ContaBancaria
{
    public int Id { get; set; }
    public User Titular { get; set; } = new User();
    [JsonInclude]
    public decimal Saldo { get; private set; }
    public List<Transacao> HistoricoTransacoes { get; set; } = new List<Transacao>();
    public ContaBancaria() {}
    public ContaBancaria(User titular)
    {
        Titular = titular;
        Saldo = 0;
        HistoricoTransacoes = new List<Transacao>();
    }
    public void Depositar(decimal valor, string descricao)
    {
        Saldo += valor;
        var transacao = new Transacao
        {
            Id = HistoricoTransacoes.Count + 1,
            Valor = valor,
            Data = DateTime.Now,
            Descricao = descricao
        };
        HistoricoTransacoes.Add(transacao);
    }
    public virtual void Sacar(decimal valor, string descricao)
    {
        if (valor > Saldo){
            Console.WriteLine("Saldo insuficiente.");
            return;
            }
        ExecutarSaque(valor, descricao);
    }
    protected void ExecutarSaque(decimal valor, string descricao)
    {
        Saldo -= valor;
        var transacao = new Transacao
        {
            Id = HistoricoTransacoes.Count + 1,
            Valor = -valor,
            Data = DateTime.Now,
            Descricao = descricao
        };
        HistoricoTransacoes.Add(transacao);
    }


    public void ExibirExtrato()
    {
        Console.WriteLine($"Extrato da Conta de {Titular.Nome}:");
        foreach (var transacao in HistoricoTransacoes)
        {
            Console.WriteLine($"{transacao.Data}: {transacao.Descricao} - {transacao.Valor:C}");
        }
    }
    public void DespesaMensal(decimal valor, string descricao)
    {
        Sacar(valor, descricao);
    } 
    public void Transferir (ContaBancaria contaDestino, decimal valor, string descricao)
    {
        this.Sacar(valor, $"Transferência para {contaDestino.Titular.Nome}: {descricao}");
        contaDestino.Depositar(valor, $"Transferência de {this.Titular.Nome}: {descricao}");
    }
}
