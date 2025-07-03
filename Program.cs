using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoilGuardSensorSim
{
    public class Program
    {
        static List<Area> areas = new List<Area>();
        static List<LeituraSensor> historicoLeituras = new List<LeituraSensor>();
        const string arquivoLeituras = "historico_leituras.txt";

        public static void Main(string[] args)
        {
            Console.WriteLine("=== SoilGuard SensorSim ===");

            CarregarLeiturasDeArquivo();

            bool rodando = true;
            while (rodando)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1 - Cadastrar nova área");
                Console.WriteLine("2 - Listar áreas cadastradas");
                Console.WriteLine("3 - Simular leitura para área");
                Console.WriteLine("4 - Mostrar histórico de leituras");
                Console.WriteLine("5 - Mostrar relatório com médias das leituras");
                Console.WriteLine("0 - Sair");
                Console.Write("Escolha uma opção: ");

                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        CadastrarArea();
                        break;
                    case "2":
                        ListarAreas();
                        break;
                    case "3":
                        SimularLeituraArea();
                        break;
                    case "4":
                        MostrarHistorico();
                        break;
                    case "5":
                        MostrarRelatorioMedias();
                        break;
                    case "0":
                        rodando = false;
                        SalvarLeiturasEmArquivo();
                        break;
                    default:
                        Console.WriteLine("Opção inválida, tente novamente.");
                        break;
                }
            }

            Console.WriteLine("Programa encerrado. Obrigada por usar o SoilGuard SensorSim!");
        }

        static void CadastrarArea()
        {
            Console.Write("Digite o nome da área: ");
            string nome = Console.ReadLine();

            Console.Write("Digite o tamanho em hectares: ");
            if (!double.TryParse(Console.ReadLine(), out double tamanho))
            {
                Console.WriteLine("Valor inválido para tamanho.");
                return;
            }

            Console.Write("Digite a cultura plantada: ");
            string cultura = Console.ReadLine();

            int novoId = areas.Count + 1;
            areas.Add(new Area(novoId, nome, tamanho, cultura));
            Console.WriteLine($"Área '{nome}' cadastrada com sucesso!");
        }

        static void ListarAreas()
        {
            if (areas.Count == 0)
            {
                Console.WriteLine("Nenhuma área cadastrada.");
                return;
            }

            Console.WriteLine("Áreas cadastradas:");
            foreach (var area in areas)
            {
                Console.WriteLine($"ID: {area.Id} - Nome: {area.Nome}, Tamanho: {area.TamanhoHectares} ha, Cultura: {area.Cultura}");
            }
        }

        static void SimularLeituraArea()
        {
            if (areas.Count == 0)
            {
                Console.WriteLine("Nenhuma área cadastrada para simular leitura.");
                return;
            }

            Console.Write("Digite o ID da área para simular leitura: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var area = areas.Find(a => a.Id == id);
            if (area == null)
            {
                Console.WriteLine("Área não encontrada.");
                return;
            }

            var leitura = SimularLeitura(area.Id);
            historicoLeituras.Add(leitura);

            Console.WriteLine($"\nLeitura sensores para área {area.Nome} em {leitura.DataLeitura}");
            Console.WriteLine($"pH: {leitura.PH} - {DiagnosticoPH(leitura.PH)}");
            Console.WriteLine($"Umidade: {leitura.Umidade}% - {DiagnosticoUmidade(leitura.Umidade)}");
            Console.WriteLine($"Temperatura: {leitura.Temperatura}°C");
            Console.WriteLine($"Nitrogênio: {leitura.Nitrogenio} mg/kg - {DiagnosticoNitrogenio(leitura.Nitrogenio)}");
            Console.WriteLine($"Fósforo: {leitura.Fosforo} mg/kg - {DiagnosticoFosforo(leitura.Fosforo)}");
            Console.WriteLine($"Potássio: {leitura.Potassio} mg/kg - {DiagnosticoPotassio(leitura.Potassio)}");
            Console.WriteLine($"Compactação: {leitura.Compactacao} kPa - {DiagnosticoCompactacao(leitura.Compactacao)}");
        }

        static void MostrarHistorico()
        {
            if (historicoLeituras.Count == 0)
            {
                Console.WriteLine("Nenhuma leitura simulada até agora.");
                return;
            }

            Console.WriteLine("Histórico de leituras:");
            foreach (var leitura in historicoLeituras)
            {
                var area = areas.Find(a => a.Id == leitura.AreaId);
                string nomeArea = area != null ? area.Nome : "Área desconhecida";

                Console.WriteLine($"\nÁrea: {nomeArea} | Data: {leitura.DataLeitura}");
                Console.WriteLine($"pH: {leitura.PH} - {DiagnosticoPH(leitura.PH)}");
                Console.WriteLine($"Umidade: {leitura.Umidade}% - {DiagnosticoUmidade(leitura.Umidade)}");
                Console.WriteLine($"Temperatura: {leitura.Temperatura}°C");
                Console.WriteLine($"Nitrogênio: {leitura.Nitrogenio} mg/kg - {DiagnosticoNitrogenio(leitura.Nitrogenio)}");
                Console.WriteLine($"Fósforo: {leitura.Fosforo} mg/kg - {DiagnosticoFosforo(leitura.Fosforo)}");
                Console.WriteLine($"Potássio: {leitura.Potassio} mg/kg - {DiagnosticoPotassio(leitura.Potassio)}");
                Console.WriteLine($"Compactação: {leitura.Compactacao} kPa - {DiagnosticoCompactacao(leitura.Compactacao)}");
            }
        }

        static void MostrarRelatorioMedias()
        {
            if (historicoLeituras.Count == 0)
            {
                Console.WriteLine("Nenhuma leitura para calcular médias.");
                return;
            }

            var grupos = historicoLeituras.GroupBy(l => l.AreaId);

            Console.WriteLine("Relatório de médias das leituras por área:");

            foreach (var grupo in grupos)
            {
                var area = areas.Find(a => a.Id == grupo.Key);
                string nomeArea = area != null ? area.Nome : "Área desconhecida";

                Console.WriteLine($"\nÁrea: {nomeArea}");
                Console.WriteLine($"Média pH: {grupo.Average(l => l.PH):F2}");
                Console.WriteLine($"Média Umidade: {grupo.Average(l => l.Umidade):F1}%");
                Console.WriteLine($"Média Temperatura: {grupo.Average(l => l.Temperatura):F1}°C");
                Console.WriteLine($"Média Nitrogênio: {grupo.Average(l => l.Nitrogenio):F1} mg/kg");
                Console.WriteLine($"Média Fósforo: {grupo.Average(l => l.Fosforo):F1} mg/kg");
                Console.WriteLine($"Média Potássio: {grupo.Average(l => l.Potassio):F1} mg/kg");
                Console.WriteLine($"Média Compactação: {grupo.Average(l => l.Compactacao):F1} kPa");
            }
        }

        public static LeituraSensor SimularLeitura(int areaId)
        {
            Random rand = new Random();

            var leitura = new LeituraSensor(areaId)
            {
                PH = Math.Round(4.5 + rand.NextDouble() * 3.0, 2),
                Umidade = Math.Round(10 + rand.NextDouble() * 40, 1),
                Temperatura = Math.Round(15 + rand.NextDouble() * 20, 1),
                Nitrogenio = Math.Round(10 + rand.NextDouble() * 50, 1),
                Fosforo = Math.Round(5 + rand.NextDouble() * 40, 1),
                Potassio = Math.Round(10 + rand.NextDouble() * 70, 1),
                Compactacao = Math.Round(200 + rand.NextDouble() * 300, 1)
            };

            return leitura;
        }

        static string DiagnosticoPH(double ph)
        {
            if (ph < 5.5) return "pH baixo, recomenda calagem";
            if (ph > 7.0) return "pH alto, cuidado com alcalinidade";
            return "pH dentro do ideal";
        }

        static string DiagnosticoUmidade(double umidade)
        {
            if (umidade < 20) return "Umidade baixa, risco de estresse hídrico";
            if (umidade > 45) return "Umidade alta, pode haver risco de fungos";
            return "Umidade adequada";
        }

        static string DiagnosticoNitrogenio(double n)
        {
            if (n < 20) return "Nitrogênio baixo, considerar adubação";
            return "Nitrogênio adequado";
        }

        static string DiagnosticoFosforo(double p)
        {
            if (p < 10) return "Fósforo baixo, pode limitar crescimento";
            return "Fósforo adequado";
        }

        static string DiagnosticoPotassio(double k)
        {
            if (k < 15) return "Potássio baixo, pode afetar resistência";
            return "Potássio adequado";
        }

        static string DiagnosticoCompactacao(double comp)
        {
            if (comp > 400) return "Compactação alta, pode prejudicar raízes";
            return "Compactação adequada";
        }

        static void SalvarLeiturasEmArquivo()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(arquivoLeituras))
                {
                    foreach (var leitura in historicoLeituras)
                    {
                        sw.WriteLine($"{leitura.AreaId};{leitura.DataLeitura};{leitura.PH};{leitura.Umidade};{leitura.Temperatura};{leitura.Nitrogenio};{leitura.Fosforo};{leitura.Potassio};{leitura.Compactacao}");
                    }
                }
                Console.WriteLine("Histórico de leituras salvo no arquivo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao salvar arquivo: " + ex.Message);
            }
        }

        static void CarregarLeiturasDeArquivo()
        {
            if (!File.Exists(arquivoLeituras))
                return;

            try
            {
                string[] linhas = File.ReadAllLines(arquivoLeituras);
                foreach (string linha in linhas)
                {
                    var partes = linha.Split(';');
                    if (partes.Length == 9)
                    {
                        int areaId = int.Parse(partes[0]);
                        DateTime data = DateTime.Parse(partes[1]);
                        double ph = double.Parse(partes[2]);
                        double umidade = double.Parse(partes[3]);
                        double temperatura = double.Parse(partes[4]);
                        double nitrogenio = double.Parse(partes[5]);
                        double fosforo = double.Parse(partes[6]);
                        double potassio = double.Parse(partes[7]);
                        double compactacao = double.Parse(partes[8]);

                        var leitura = new LeituraSensor(areaId)
                        {
                            DataLeitura = data,
                            PH = ph,
                            Umidade = umidade,
                            Temperatura = temperatura,
                            Nitrogenio = nitrogenio,
                            Fosforo = fosforo,
                            Potassio = potassio,
                            Compactacao = compactacao
                        };

                        historicoLeituras.Add(leitura);
                    }
                }
                Console.WriteLine("Histórico de leituras carregado do arquivo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao carregar arquivo: " + ex.Message);
            }
        }
    }

    public class Area
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public double TamanhoHectares { get; set; }
        public string Cultura { get; set; }

        public Area(int id, string nome, double tamanho, string cultura)
        {
            Id = id;
            Nome = nome;
            TamanhoHectares = tamanho;
            Cultura = cultura;
        }
    }

    public class LeituraSensor
    {
        public int AreaId { get; set; }
        public DateTime DataLeitura { get; set; }
        public double PH { get; set; }
        public double Umidade { get; set; }
        public double Temperatura { get; set; }
        public double Nitrogenio { get; set; }
        public double Fosforo { get; set; }
        public double Potassio { get; set; }
        public double Compactacao { get; set; }

        public LeituraSensor(int areaId)
        {
            AreaId = areaId;
            DataLeitura = DateTime.Now;
        }
    }
}

