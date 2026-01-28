namespace models;
using System.Text.Json;
using System.Text.Json.Serialization;
public class Banco
{
    public string NomeDoBanco { get; set; } = "Banco Padrão";
    public List<ContaBancaria> Contas { get; set; }= new List<ContaBancaria>();
    public void AdicionarConta(ContaBancaria conta)
    {
        conta.Id = Contas.Count + 1;
        Contas.Add(conta);
    }
    public ContaBancaria ? BuscarContaPorId(int id)
    {
        return Contas.Find(c => c.Id == id);
    }
    public ContaBancaria ? BuscarContaPorTitular(string nome)
    {
        return Contas.Find(c => c.Titular.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }

    public void SalvarEmArquivo(string caminho)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(caminho, json);
        Console.WriteLine($"\n>> Banco de dados salvo em '{caminho}' com sucesso.");
    }
    // Método ESTÁTICO: Você chama ele sem ter uma instância do banco (Banco.CarregarDoArquivo)
    public static Banco CarregarDoArquivo(string caminho)
    {
        if (File.Exists(caminho))
        {
            string json = File.ReadAllText(caminho);
            // O '!' silencia o warning de nulo
            Banco bancoCarregado = JsonSerializer.Deserialize<Banco>(json)!;
            
            // Re-hidratar lógica: recalcular saldos se necessário, ou apenas retornar
            Console.WriteLine(">> Dados do banco carregados do arquivo!");
            return bancoCarregado;
        }
        else
        {
            Console.WriteLine(">> Arquivo não encontrado. Criando um novo Banco vazio.");
            return new Banco { NomeDoBanco = "Meu Banco Novo" };
        }
    }

}
