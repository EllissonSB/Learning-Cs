using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using models;



class Program
{
    static void Main(string[] args)
    {
        string arquivoDb = "dados_banco.json";
        Banco banco = Banco.CarregarDoArquivo(arquivoDb);
        
        ContaBancaria ? contaAtual = null;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== {banco.NomeDoBanco} ===");
            if (contaAtual == null)
            {
                Console.WriteLine("1. Criar Nova Conta");
                Console.WriteLine("2. Entrar em Conta Existente");
                Console.WriteLine("3. Listar Todas as Contas (Admin)");
                Console.WriteLine("0. Sair e Salvar");
            }
            else
            {
                Console.WriteLine($"\nLogado como: {contaAtual.Titular.Nome} | Saldo: {contaAtual.Saldo:C}");
                Console.WriteLine("1. Depositar");
                Console.WriteLine("2. Sacar");
                Console.WriteLine("3. Transferir");
                Console.WriteLine("4. Ver Extrato");
                Console.WriteLine("0. Deslogar (Voltar ao menu principal)");
            }
            Console.Write("\nEscolha uma opção: ");
            string escolha = Console.ReadLine() ?? "";

            if(contaAtual == null)
            {
                switch(escolha)
                {
                    case "1":
                        criarConta(banco);
                        break;
                    case "2":
                        contaAtual = LogarConta(banco);
                        break;
                    case "3":
                        ListarContas(banco);
                        break;
                    case "0":
                        banco.SalvarEmArquivo(arquivoDb);
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Pressione Enter para continuar...");
                        break;
                }
            }

            else
            {
                switch(escolha)
                {
                    case "1":
                        Console.Write("Valor para depositar: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal valorDeposito) && valorDeposito > 0)
                        {
                            contaAtual.Depositar(valorDeposito, "Depósito via terminal");
                            Console.WriteLine("Sucesso!");
                        }
                        else
                        {
                            Console.WriteLine("Valor inválido! Digite apenas números.");
                        }
                        Console.ReadKey();
                        break;
                    case "2":
                        Console.Write("Valor para sacar: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal valorSaque) && valorSaque > 0)
                        {
                            contaAtual.Sacar(valorSaque, "Saque via terminal");
                            Console.WriteLine("Sucesso!");
                        }
                        else
                        {
                            Console.WriteLine("Valor inválido! Digite apenas números.");
                        }
                        Console.ReadKey();
                        break;
                    case "3":
                        Console.Write("ID da conta destino: ");
                        if (int.TryParse(Console.ReadLine(), out int idDestino))
                        {
                            var contaDestino = banco.BuscarContaPorId(idDestino);
                            if (contaDestino != null)
                            {
                                Console.Write("Valor para transferir: ");
                                if (decimal.TryParse(Console.ReadLine(), out decimal valorTransferencia) && valorTransferencia > 0)
                                {
                                    contaAtual.Transferir(contaDestino, valorTransferencia, "Transferência via terminal");
                                    Console.WriteLine("Sucesso!");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    Console.WriteLine("Valor inválido! Digite apenas números.");
                                    Console.ReadKey();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Conta destino não encontrada.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("ID inválido! Digite apenas números.");
                            Console.ReadKey();
                        }
                        break;
                    case "4":
                        contaAtual.ExibirExtrato();
                        Console.WriteLine("Pressione Enter para continuar...");
                        Console.ReadKey();
                        break;
                    case "0":
                        contaAtual = null;
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Pressione Enter para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    static void criarConta(Banco banco)
    {   
        //Dados para o titular
        Console.Write("Nome do Titular:");
        string nome = Console.ReadLine() ?? "";
        Console.Write("Email do Titular:");
        string email = Console.ReadLine() ?? "";
        Console.Write("Senha:");
        string senha = Console.ReadLine() ?? "";
        string senhaHash = Security.HashPassword(senha);
        var usuario = new User
        {
            Id = banco.Contas.Count + 1,
            Nome = nome,
            Email = email,
            SenhaHash = senhaHash
        };
        // --- ESCOLHA DO TIPO DE CONTA ---
        Console.WriteLine("\nQual tipo de conta deseja abrir?");
        Console.WriteLine("1. Conta Poupança (Sem taxas, rende juros)");
        Console.WriteLine("2. Conta Corrente (Taxa de R$ 5.00 por saque)");
        Console.Write("Opção: ");
        string tipo = Console.ReadLine() ?? "";
        ContaBancaria novaConta;
        if (tipo == "1")
        {
            novaConta = new ContaPoupanca(usuario);
        }
        else
        {
            novaConta = new ContaCorrente(usuario);
        }
        banco.AdicionarConta(novaConta);
        Console.WriteLine($"\n>> Conta criada com sucesso! ID da Conta: {novaConta.Id}");
        Console.WriteLine("Pressione Enter para continuar...");
        Console.ReadKey();
    }

    static ContaBancaria ? LogarConta(Banco banco)
    {
        Console.Write("Digite o nome do titular da conta: ");
        var nome = Console.ReadLine() ?? "";
        var conta = banco.BuscarContaPorTitular(nome);
        if (conta != null)
        {
            Console.Write("Digite a senha: ");
            string senha = Console.ReadLine() ?? "";
            string senhaHash = Security.HashPassword(senha);
            if (conta.Titular.SenhaHash != senhaHash)
            {
                Console.WriteLine(">> Senha incorreta. Pressione Enter para continuar...");
                Console.ReadKey();
                return null;
            }
            Console.Write(">> Login bem-sucedido! Pressione Enter para continuar...");
            Console.ReadKey();
            return conta;
        }
        else 
        {
            Console.WriteLine(">> Conta não encontrada. Pressione Enter para continuar...");
            Console.ReadKey();
            return null;
        }
    }

    static void ListarContas(Banco banco)
    {
        Console.WriteLine("\n=== Lista de Contas ===");
        foreach (var conta in banco.Contas)
        {
            Console.WriteLine($"ID: {conta.Id} | Titular: {conta.Titular.Nome} | Saldo: {conta.Saldo:C} | Tipo: :{conta.GetType().Name}");
        }
        Console.WriteLine("Pressione Enter para continuar...");
        Console.ReadKey();
    }

    static void MenuRelatorios(Banco banco)
    {
        Console.Clear();
        Console.WriteLine("=== RELATÓRIOS GERENCIAIS (LINQ) ===");
        
        // 1. Soma Total (Quanto dinheiro o banco tem?)
        // O LINQ 'Sum' percorre a lista e soma a propriedade Saldo
        decimal totalBanco = banco.Contas.Sum(c => c.Saldo);
        Console.WriteLine($"\n💰 Patrimônio Total do Banco: {totalBanco:C}");

        // 2. Média de Saldos
        if (banco.Contas.Any()) // Verifica se tem contas para não dividir por zero
        {
            double media = banco.Contas.Average(c => (double)c.Saldo);
            Console.WriteLine($"📊 Saldo Médio dos Clientes: {media:C2}");
        }

        // 3. Top 3 Clientes Mais Ricos
        // OrderByDescending = Ordena do maior para o menor
        // Take(3) = Pega apenas os 3 primeiros
        var topRicos = banco.Contas.OrderByDescending(c => c.Saldo).Take(3);

        Console.WriteLine("\n🏆 Top 3 Clientes Mais Ricos:");
        foreach (var c in topRicos)
        {
            // Verifica o tipo da conta para exibir bonitinho
            string tipo = c is ContaCorrente ? "Corrente" : "Poupança";
            Console.WriteLine($"- {c.Titular.Nome} ({tipo}): {c.Saldo:C}");
        }

        // 4. Clientes com Saldo Negativo (Devedores)
        // Where = Filtra a lista com base numa condição
        var devedores = banco.Contas.Where(c => c.Saldo < 0).ToList();

        Console.WriteLine($"\n⚠️ Clientes no Vermelho: {devedores.Count}");
        foreach (var c in devedores)
        {
            Console.WriteLine($"- {c.Titular.Nome}: {c.Saldo:C}");
        }

        Console.WriteLine("\nPressione Enter para voltar...");
        Console.ReadKey();
        }
}
