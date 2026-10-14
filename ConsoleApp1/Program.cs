using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

struct Videogame
{
    public string Nome;
    public string Plataforma;
    public int Quantidade;
    public decimal Preco;

    public override string ToString()
    {
        return "Nome: " + Nome +
               " | Plataforma: " + Plataforma +
               " | Estoque: " + Quantidade +
               " | Preço: " + Preco.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
    }
}

struct Acessorio
{
    public string Nome;
    public string Tipo;
    public int Quantidade;
    public decimal Preco;

    public override string ToString()
    {
        return "Nome: " + Nome +
               " | Tipo: " + Tipo +
               " | Estoque: " + Quantidade +
               " | Preço: " + Preco.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
    }
}

struct Venda
{
    public string Categoria;
    public string NomeProduto;
    public int Quantidade;
    public decimal PrecoUnitario;
    public decimal Total;
    public string DataHora;

    public override string ToString()
    {
        return DataHora + " | " + Categoria + " | " + NomeProduto +
               " | Qtd.: " + Quantidade +
               " | Unitário: " + PrecoUnitario.ToString("C2", CultureInfo.GetCultureInfo("pt-BR")) +
               " | Total: " + Total.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
    }
}

class Program
{
    static List<Videogame> videogames = new List<Videogame>();
    static List<Acessorio> acessorios = new List<Acessorio>();
    static List<Venda> vendas = new List<Venda>();

    static readonly string pastaDados = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dados");
    static readonly string arquivoVideogames = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dados", "videogames.txt");
    static readonly string arquivoAcessorios = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dados", "acessorios.txt");
    static readonly string arquivoVendas = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dados", "vendas.txt");

    static readonly CultureInfo culturaBR = CultureInfo.GetCultureInfo("pt-BR");
    static readonly CultureInfo culturaArquivo = CultureInfo.InvariantCulture;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "Game Store - Estoque e Vendas";

        PrepararArquivos();
        CarregarDados();

        bool executando = true;
        while (executando)
        {
            Console.Clear();
            Titulo("GAME STORE");
            Console.WriteLine("1  - Cadastrar produto");
            Console.WriteLine("2  - Realizar venda");
            Console.WriteLine("3  - Consultar produto");
            Console.WriteLine("4  - Excluir produto");
            Console.WriteLine("99 - Imprimir estoque");
            Console.WriteLine("1000 - Relatório de vendas");
            Console.WriteLine("0  - Sair");
            Console.WriteLine();
            string opcao = LerTexto("Escolha uma opção: ");

            switch (opcao)
            {
                case "1": MenuCadastro(); break;
                case "2": RealizarVenda(); break;
                case "3": ConsultarProduto(); break;
                case "4": ExcluirProduto(); break;
                case "99": ImprimirEstoque(); break;
                case "1000": RelatorioVendas(); break;
                case "0":
                case "0000":
                    SalvarTodosDados();
                    executando = false;
                    Console.WriteLine("Dados salvos!");
                    break;
                default:
                    Aviso("Opção inválida. Tente novamente.");
                    Pausar();
                    break;
            }
        }
    }

    static void PrepararArquivos()
    {
        if (!Directory.Exists(pastaDados))
            Directory.CreateDirectory(pastaDados);

        if (!File.Exists(arquivoVideogames)) File.WriteAllText(arquivoVideogames, "");
        if (!File.Exists(arquivoAcessorios)) File.WriteAllText(arquivoAcessorios, "");
        if (!File.Exists(arquivoVendas)) File.WriteAllText(arquivoVendas, "");
    }

    static void CarregarDados()
    {
        videogames = CarregarVideogames();
        acessorios = CarregarAcessorios();
        vendas = CarregarVendas();
    }

    static List<Videogame> CarregarVideogames()
    {
        List<Videogame> lista = new List<Videogame>();
        foreach (string linha in File.ReadAllLines(arquivoVideogames, Encoding.UTF8))
        {
            string[] p = linha.Split('|');
            if (p.Length != 4) continue;

            int quantidade;
            decimal preco;
            if (int.TryParse(p[2], out quantidade) &&
                decimal.TryParse(p[3], NumberStyles.Number, culturaArquivo, out preco))
            {
                Videogame item = new Videogame();
                item.Nome = DescodificarTexto(p[0]);
                item.Plataforma = DescodificarTexto(p[1]);
                item.Quantidade = quantidade;
                item.Preco = preco;
                lista.Add(item);
            }
        }
        return lista;
    }

    static List<Acessorio> CarregarAcessorios()
    {
        List<Acessorio> lista = new List<Acessorio>();
        foreach (string linha in File.ReadAllLines(arquivoAcessorios, Encoding.UTF8))
        {
            string[] p = linha.Split('|');
            if (p.Length != 4) continue;

            int quantidade;
            decimal preco;
            if (int.TryParse(p[2], out quantidade) &&
                decimal.TryParse(p[3], NumberStyles.Number, culturaArquivo, out preco))
            {
                Acessorio item = new Acessorio();
                item.Nome = DescodificarTexto(p[0]);
                item.Tipo = DescodificarTexto(p[1]);
                item.Quantidade = quantidade;
                item.Preco = preco;
                lista.Add(item);
            }
        }
        return lista;
    }

    static List<Venda> CarregarVendas()
    {
        List<Venda> lista = new List<Venda>();
        foreach (string linha in File.ReadAllLines(arquivoVendas, Encoding.UTF8))
        {
            string[] p = linha.Split('|');
            if (p.Length != 6) continue;

            int quantidade;
            decimal preco;
            decimal total;
            if (int.TryParse(p[2], out quantidade) &&
                decimal.TryParse(p[3], NumberStyles.Number, culturaArquivo, out preco) &&
                decimal.TryParse(p[4], NumberStyles.Number, culturaArquivo, out total))
            {
                Venda venda = new Venda();
                venda.Categoria = DescodificarTexto(p[0]);
                venda.NomeProduto = DescodificarTexto(p[1]);
                venda.Quantidade = quantidade;
                venda.PrecoUnitario = preco;
                venda.Total = total;
                venda.DataHora = DescodificarTexto(p[5]);
                lista.Add(venda);
            }
        }
        return lista;
    }

    static void SalvarVideogames()
    {
        List<string> linhas = new List<string>();
        foreach (Videogame item in videogames)
            linhas.Add(CodificarTexto(item.Nome) + "|" + CodificarTexto(item.Plataforma) + "|" +
                       item.Quantidade + "|" + item.Preco.ToString(culturaArquivo));
        File.WriteAllLines(arquivoVideogames, linhas.ToArray(), Encoding.UTF8);
    }

    static void SalvarAcessorios()
    {
        List<string> linhas = new List<string>();
        foreach (Acessorio item in acessorios)
            linhas.Add(CodificarTexto(item.Nome) + "|" + CodificarTexto(item.Tipo) + "|" +
                       item.Quantidade + "|" + item.Preco.ToString(culturaArquivo));
        File.WriteAllLines(arquivoAcessorios, linhas.ToArray(), Encoding.UTF8);
    }

    static void SalvarVendas()
    {
        List<string> linhas = new List<string>();
        foreach (Venda venda in vendas)
            linhas.Add(CodificarTexto(venda.Categoria) + "|" + CodificarTexto(venda.NomeProduto) + "|" +
                       venda.Quantidade + "|" + venda.PrecoUnitario.ToString(culturaArquivo) + "|" +
                       venda.Total.ToString(culturaArquivo) + "|" + CodificarTexto(venda.DataHora));
        File.WriteAllLines(arquivoVendas, linhas.ToArray(), Encoding.UTF8);
    }

    static void SalvarTodosDados()
    {
        SalvarVideogames();
        SalvarAcessorios();
        SalvarVendas();
    }

    static string CodificarTexto(string texto)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(texto ?? ""));
    }

    static string DescodificarTexto(string texto)
    {
        try { return Encoding.UTF8.GetString(Convert.FromBase64String(texto)); }
        catch { return texto; }
    }

    static void MenuCadastro()
    {
        Console.Clear();
        Titulo("CADASTRAR PRODUTO");
        Console.WriteLine("1 - Videogame");
        Console.WriteLine("2 - Acessório");
        Console.WriteLine("0 - Voltar");
        string opcao = LerTexto("Tipo de produto: ");

        if (opcao == "1") CadastrarVideogame();
        else if (opcao == "2") CadastrarAcessorio();
        else if (opcao != "0") Aviso("Opção inválida.");

        if (opcao != "0") Pausar();
    }

    static void CadastrarVideogame()
    {
        Console.Clear();
        Titulo("CADASTRO DE VIDEOGAME");
        Videogame item = new Videogame();
        item.Nome = LerTexto("Nome do videogame: ");
        if (ExisteVideogame(item.Nome))
        {
            Aviso("Já existe um videogame com esse nome.");
            return;
        }
        item.Plataforma = LerTexto("Plataforma/versão (ex.: PS5, Xbox Series S): ");
        item.Quantidade = LerInteiro("Quantidade inicial em estoque: ", 0, int.MaxValue);
        item.Preco = LerDecimal("Preço unitário (R$): ", 0.01m, 100000000m);
        videogames.Add(item);
        SalvarVideogames();
        Sucesso("Videogame cadastrado e salvo!");
    }

    static void CadastrarAcessorio()
    {
        Console.Clear();
        Titulo("CADASTRO DE ACESSÓRIO");
        Acessorio item = new Acessorio();
        item.Nome = LerTexto("Nome do acessório: ");
        if (ExisteAcessorio(item.Nome))
        {
            Aviso("Já existe um acessório com esse nome.");
            return;
        }
        item.Tipo = LerTexto("Tipo (ex.: controle, headset, teclado): ");
        item.Quantidade = LerInteiro("Quantidade inicial em estoque: ", 0, int.MaxValue);
        item.Preco = LerDecimal("Preço unitário (R$): ", 0.01m, 100000000m);
        acessorios.Add(item);
        SalvarAcessorios();
        Sucesso("Acessório cadastrado e salvo!");
    }

    static bool ExisteVideogame(string nome)
    {
        foreach (Videogame item in videogames)
            if (item.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    static bool ExisteAcessorio(string nome)
    {
        foreach (Acessorio item in acessorios)
            if (item.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    static void ConsultarProduto()
    {
        Console.Clear();
        Titulo("CONSULTAR PRODUTO");
        string nome = LerTexto("Digite o nome (ou parte do nome): ");
        bool encontrou = false;

        foreach (Videogame item in videogames)
        {
            if (item.Nome.IndexOf(nome, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Console.WriteLine("[VIDEOGAME] " + item);
                encontrou = true;
            }
        }
        foreach (Acessorio item in acessorios)
        {
            if (item.Nome.IndexOf(nome, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Console.WriteLine("[ACESSÓRIO] " + item);
                encontrou = true;
            }
        }

        if (!encontrou) Aviso("Nenhum produto encontrado.");
        Pausar();
    }

    static void ExcluirProduto()
    {
        Console.Clear();
        Titulo("EXCLUIR PRODUTO");
        Console.WriteLine("1 - Videogame");
        Console.WriteLine("2 - Acessório");
        Console.WriteLine("0 - Voltar");
        string tipo = LerTexto("Categoria: ");
        if (tipo == "0") return;

        string nome = LerTexto("Nome exato do produto: ");
        if (tipo == "1")
        {
            int indice = -1;
            for (int i = 0; i < videogames.Count; i++)
                if (videogames[i].Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)) { indice = i; break; }

            if (indice < 0) Aviso("Videogame não encontrado.");
            else
            {
                Console.WriteLine(videogames[indice]);
                if (Confirmar("Confirma a exclusão? (S/N): "))
                {
                    videogames.RemoveAt(indice);
                    SalvarVideogames();
                    Sucesso("Videogame excluído.");
                }
                else Console.WriteLine("Exclusão cancelada.");
            }
        }
        else if (tipo == "2")
        {
            int indice = -1;
            for (int i = 0; i < acessorios.Count; i++)
                if (acessorios[i].Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)) { indice = i; break; }

            if (indice < 0) Aviso("Acessório não encontrado.");
            else
            {
                Console.WriteLine(acessorios[indice]);
                if (Confirmar("Confirma a exclusão? (S/N): "))
                {
                    acessorios.RemoveAt(indice);
                    SalvarAcessorios();
                    Sucesso("Acessório excluído.");
                }
                else Console.WriteLine("Exclusão cancelada.");
            }
        }
        else Aviso("Categoria inválida.");

        Pausar();
    }

    static void RealizarVenda()
    {
        Console.Clear();
        Titulo("REALIZAR VENDA");
        Console.WriteLine("1 - Videogame");
        Console.WriteLine("2 - Acessório");
        Console.WriteLine("0 - Voltar");
        string tipo = LerTexto("Categoria: ");

        if (tipo == "1") VenderVideogame();
        else if (tipo == "2") VenderAcessorio();
        else if (tipo != "0") Aviso("Categoria inválida.");

        if (tipo != "0") Pausar();
    }

    static void VenderVideogame()
    {
        if (videogames.Count == 0) { Aviso("Não há videogames cadastrados."); return; }
        ImprimirVideogames();
        string nome = LerTexto("Nome exato do videogame: ");
        int indice = -1;
        for (int i = 0; i < videogames.Count; i++)
            if (videogames[i].Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)) { indice = i; break; }

        if (indice < 0) { Aviso("Videogame não encontrado."); return; }
        Videogame item = videogames[indice];
        Console.WriteLine("Selecionado: " + item);
        int quantidade = LerInteiro("Quantidade para vender: ", 1, int.MaxValue);
        if (quantidade > item.Quantidade) { Aviso("Estoque insuficiente. Disponível: " + item.Quantidade); return; }

        decimal total = item.Preco * quantidade;
        Console.WriteLine("Total da venda: " + total.ToString("C2", culturaBR));
        if (!Confirmar("Confirmar venda? (S/N): ")) { Console.WriteLine("Venda cancelada."); return; }

        item.Quantidade -= quantidade;
        videogames[indice] = item;
        Venda venda = new Venda();
        venda.Categoria = "Videogame";
        venda.NomeProduto = item.Nome;
        venda.Quantidade = quantidade;
        venda.PrecoUnitario = item.Preco;
        venda.Total = total;
        venda.DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        vendas.Add(venda);
        SalvarVideogames();
        SalvarVendas();
        Sucesso("Venda registrada e estoque atualizado!");
    }

    static void VenderAcessorio()
    {
        if (acessorios.Count == 0) { Aviso("Não há acessórios cadastrados."); return; }
        ImprimirAcessorios();
        string nome = LerTexto("Nome exato do acessório: ");
        int indice = -1;
        for (int i = 0; i < acessorios.Count; i++)
            if (acessorios[i].Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)) { indice = i; break; }

        if (indice < 0) { Aviso("Acessório não encontrado."); return; }
        Acessorio item = acessorios[indice];
        Console.WriteLine("Selecionado: " + item);
        int quantidade = LerInteiro("Quantidade para vender: ", 1, int.MaxValue);
        if (quantidade > item.Quantidade) { Aviso("Estoque insuficiente. Disponível: " + item.Quantidade); return; }

        decimal total = item.Preco * quantidade;
        Console.WriteLine("Total da venda: " + total.ToString("C2", culturaBR));
        if (!Confirmar("Confirmar venda? (S/N): ")) { Console.WriteLine("Venda cancelada."); return; }

        item.Quantidade -= quantidade;
        acessorios[indice] = item;
        Venda venda = new Venda();
        venda.Categoria = "Acessório";
        venda.NomeProduto = item.Nome;
        venda.Quantidade = quantidade;
        venda.PrecoUnitario = item.Preco;
        venda.Total = total;
        venda.DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        vendas.Add(venda);
        SalvarAcessorios();
        SalvarVendas();
        Sucesso("Venda registrada e estoque atualizado!");
    }

    static void ImprimirEstoque()
    {
        Console.Clear();
        Titulo("ESTOQUE ATUAL");
        ImprimirVideogames();
        ImprimirAcessorios();
        Pausar();
    }

    static void ImprimirVideogames()
    {
        Console.WriteLine();
        Console.WriteLine("--- VIDEOGAMES ---");
        if (videogames.Count == 0) Console.WriteLine("Nenhum videogame cadastrado.");
        foreach (Videogame item in videogames) Console.WriteLine(item.ToString());
    }

    static void ImprimirAcessorios()
    {
        Console.WriteLine();
        Console.WriteLine("--- ACESSÓRIOS ---");
        if (acessorios.Count == 0) Console.WriteLine("Nenhum acessório cadastrado.");
        foreach (Acessorio item in acessorios) Console.WriteLine(item.ToString());
    }

    static void RelatorioVendas()
    {
        Console.Clear();
        Titulo("RELATÓRIO DE VENDAS");
        decimal totalVideogames = 0m;
        decimal totalAcessorios = 0m;
        decimal totalGeral = 0m;

        if (vendas.Count == 0) Console.WriteLine("Ainda não há vendas registradas.");
        else
        {
            foreach (Venda venda in vendas)
            {
                Console.WriteLine(venda.ToString());
                if (venda.Categoria == "Videogame") totalVideogames += venda.Total;
                else if (venda.Categoria == "Acessório") totalAcessorios += venda.Total;
                totalGeral += venda.Total;
            }
        }

        Console.WriteLine();
        Console.WriteLine("Total vendido em videogames: " + totalVideogames.ToString("C2", culturaBR));
        Console.WriteLine("Total vendido em acessórios: " + totalAcessorios.ToString("C2", culturaBR));
        Console.WriteLine("TOTAL GERAL: " + totalGeral.ToString("C2", culturaBR));
        Pausar();
    }

    static string LerTexto(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string texto = (Console.ReadLine() ?? "").Trim();
            if (texto.Length > 0 && texto.IndexOf('|') < 0) return texto;
            Aviso("Digite um valor válido. O campo não pode ficar vazio nem conter '|'.");
        }
    }

    static int LerInteiro(string mensagem, int minimo, int maximo)
    {
        while (true)
        {
            Console.Write(mensagem);
            int valor;
            if (int.TryParse(Console.ReadLine(), out valor) && valor >= minimo && valor <= maximo)
                return valor;
            Aviso("Digite um número inteiro válido (mínimo " + minimo + ").");
        }
    }

    static decimal LerDecimal(string mensagem, decimal minimo, decimal maximo)
    {
        while (true)
        {
            Console.Write(mensagem);
            string entrada = (Console.ReadLine() ?? "").Trim().Replace("R$", "").Trim();
            decimal valor;
            bool valido = decimal.TryParse(entrada, NumberStyles.Number, culturaBR, out valor)
                          || decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);
            if (valido && valor >= minimo && valor <= maximo) return valor;
            Aviso("Digite um preço válido maior que zero. Ex.: 129,90");
        }
    }

    static bool Confirmar(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string resposta = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
            if (resposta == "S" || resposta == "SIM") return true;
            if (resposta == "N" || resposta == "NAO" || resposta == "NÃO") return false;
            Aviso("Responda S para sim ou N para não.");
        }
    }

    static void Titulo(string texto)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==============================================");
        Console.WriteLine("  " + texto);
        Console.WriteLine("==============================================");
        Console.ResetColor();
    }

    static void Sucesso(string mensagem)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(mensagem);
        Console.ResetColor();
    }

    static void Aviso(string mensagem)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(mensagem);
        Console.ResetColor();
    }

    static void Pausar()
    {
        Console.WriteLine();
        Console.Write("Pressione ENTER para continuar...");
        Console.ReadLine();
    }
}