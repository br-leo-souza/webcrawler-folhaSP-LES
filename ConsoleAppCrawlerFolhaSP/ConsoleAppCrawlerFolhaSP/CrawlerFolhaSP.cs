using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;


public class CrawlerFolhaSP
{
	public class Arquivo
	{
		public Arquivo(string titulo,
					   string subcategoria,
					   string horario,
					   string cotacao_Toafim,
					   string cotacao_usuario,
					   string faixa_preco,
					   string wifi,
					   string valet_estacionamento,
					   string delivery,
					   string reserva,
					   string descricao_pequena,
					   string materia,
					   string endereco,
					   string site,
					   string mapa,
					   string url_imagem)
		{
			this.titulo = titulo.Replace("?", " ").TrimStart().TrimEnd(); ;
			this.subcategoria = subcategoria.TrimStart().TrimEnd();
			this.horario = horario.TrimStart().TrimEnd();
			this.cotacao_Toafim = cotacao_Toafim.TrimStart().TrimEnd();
			this.cotacao_usuario = cotacao_usuario.TrimStart().TrimEnd();
			this.faixa_preco = faixa_preco.Trim();
			this.wifi = wifi.TrimStart().TrimEnd();
			this.valet_estacionamento = valet_estacionamento.TrimStart().TrimEnd();
			this.delivery = delivery.TrimStart().TrimEnd();
			this.reserva = reserva.TrimStart().TrimEnd();
			this.descricao_pequena = descricao_pequena.TrimStart().TrimEnd();
			this.materia = materia.TrimStart().TrimEnd();
			this.endereco = endereco.TrimStart().TrimEnd();
			this.site = site.TrimStart().TrimEnd();
			this.mapa = mapa.Trim().Substring(0, 11) + mapa.Trim().Substring(30);
			this.url_imagem = url_imagem.TrimStart().TrimEnd();
		}

		public Arquivo(string url_interna)
		{
			this.url_interna = url_interna;
		}

		public string titulo { get; set; }
		public string subcategoria { get; set; }
		public string horario { get; set; }
		public string cotacao_Toafim { get; set; }
		public string cotacao_usuario { get; set; }
		public string faixa_preco { get; set; }
		public string wifi { get; set; }
		public string valet_estacionamento { get; set; }
		public string delivery { get; set; }
		public string reserva { get; set; }
		public string descricao_pequena { get; set; }
		public string materia { get; set; }
		public string endereco { get; set; }
		public string site { get; set; }
		public string mapa { get; set; }
		public string url_imagem { get; set; }
		public string url_interna { get; set; }
	}

	public static void Main(string[] args)
	{
		Console.WriteLine("Início do crawler");

		DateTime hora_inicial = DateTime.Now;
		Console.WriteLine("Hora início: " + hora_inicial.TimeOfDay);

		List<Arquivo> listaArquivos = new List<Arquivo>();
		string pagina = "";
		int contador = 1;
		string URL = "http://guia.folha.uol.com.br/busca/restaurantes?page=";

		Console.WriteLine("\n");
		Console.WriteLine("Acessando página 1");
		pagina = AcessarUrl(URL + contador.ToString());

		string startDelimiter = "Filtrar</button>";
		string listStartPosition = "<li class";
		string listEndPosition = "</li>";

		Console.WriteLine("Recuperando lista da página 1");
		listaArquivos = ExtractString(startDelimiter, listStartPosition, listEndPosition, pagina);

		Console.WriteLine("Gerando arquivos da página 1");
		GeraArquivos(listaArquivos);

		do
		{
			listaArquivos = new List<Arquivo>();

			contador++;

			Console.WriteLine("\n");
			Console.WriteLine("Acessando página:" + contador.ToString());
			pagina = AcessarUrl(URL + contador.ToString());

			Console.WriteLine("Recuperando lista da página:" + contador.ToString());
			listaArquivos = ExtractString(startDelimiter, listStartPosition, listEndPosition, pagina);

			Console.WriteLine("Gerando arquivos da página:" + contador.ToString());
			GeraArquivos(listaArquivos);

			if (pagina.IndexOf("Não há resultados para a busca realizada") != -1)
			{
				pagina = null;
			}
		}
		while (pagina != null);

		Console.WriteLine("Fim do crawler");

		DateTime hora_final = DateTime.Now;
		Console.WriteLine("Hora Fim: " + hora_final.TimeOfDay);
	}

	static void GeraArquivos(List<Arquivo> lista)
	{
		foreach (Arquivo arquivo in lista)
		{
			if (null != arquivo)
			{
				//Gera um id randomico
				int id = RandomNumber();

				//Path do diretório de cada restaurante
				string folder = "C:\\crawler-folhasp\\restaurantes\\" + arquivo.titulo;                                                                             //Path de diretório
				string path = folder + "\\" + arquivo.titulo.Replace(" ", "_") + "_" + id + ".txt";                                                                 //Path de cada .txt
				string path_json = folder + "\\" + arquivo.titulo.Replace(" ", "_") + "_" + id + "_json.txt";                                                       //Path de cada json .txt
				string path_image = folder + "\\" + arquivo.titulo.Replace(" ", "_") + "_" + id + arquivo.url_imagem.Substring(arquivo.url_imagem.Length - 4);      //Path de cada imagem

				GerarDiretorio(folder);               //Cria o diretório
				GerarArquivoTxt(path, arquivo);       //Método que gera o arquivo .txt
				GerarArquivoJson(arquivo, path_json); //Método que gera o arquivo json .txt
				DownloadImagem(arquivo, path_image);  //Método que executa o download da imagem caso ela exista
			}
		}
	}

	private static List<Arquivo> ExtractString(string nameDelimitar, string nameStartDelimiter, string nameEndPosition, string page)
	{
		List<Arquivo> lista = new List<Arquivo>();
		int startDelimiter = page.IndexOf(nameDelimitar);
		int contador = 1;

		while (contador < 15)
		{
			int viewStateNamePosition = startDelimiter;
			int viewStateValuePosition = page.IndexOf(nameStartDelimiter, viewStateNamePosition);

			int viewStateStartPosition = viewStateValuePosition + nameStartDelimiter.Length;
			int viewStateEndPosition = page.IndexOf(nameEndPosition, viewStateStartPosition);

			//Aqui vc vai obter o bloco com cada restaurante 
			string resultado = page.Substring(viewStateStartPosition, viewStateEndPosition - viewStateStartPosition);

			lista.Add(RetornaParametros(resultado));

			startDelimiter = viewStateEndPosition;
			contador++;
		}

		return lista;
	}

	static void GerarDiretorio(string path)
	{
		try
		{
			//Cria o diretório
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
		catch
		{
		}
	}

	static void GerarArquivoTxt(string path, Arquivo arquivo)
	{
		try
		{
			if (!File.Exists(path))
			{
				using (StreamWriter sw = File.CreateText(path))
				{
					sw.WriteLine("Título: " + arquivo.titulo);
					sw.WriteLine("\r\nGênero (subcategoria): " + arquivo.subcategoria);
					sw.WriteLine("\r\nHorário: " + arquivo.horario);
					sw.WriteLine("\r\nCotação Toafim: " + arquivo.cotacao_Toafim);
					sw.WriteLine("\r\nCotação usuário: " + arquivo.cotacao_usuario);
					sw.WriteLine("\r\nFaixa de preço dia: " + arquivo.faixa_preco);
					sw.WriteLine("\r\nFaixa de preço noite: " + arquivo.faixa_preco);
					sw.WriteLine("\r\nWifi: " + arquivo.wifi);
					sw.WriteLine("\r\nValet Park/Estacionamento: " + arquivo.valet_estacionamento);
					sw.WriteLine("\r\nEm Casa (manda para IFood): " + arquivo.delivery);
					sw.WriteLine("\r\nReservar: " + arquivo.reserva);
					sw.WriteLine("\r\nDescrição pequena: " + arquivo.descricao_pequena);
					sw.WriteLine("\r\nMatéria: " + arquivo.materia);
					sw.WriteLine("\r\nEndereço/Bairro: " + arquivo.endereco);
					sw.WriteLine("\r\nSite: " + arquivo.site);
					sw.WriteLine("\r\nMapa: " + arquivo.mapa);
					sw.Close();
				}
			}
		}
		catch
		{
		}
	}

	static void GerarArquivoJson(Arquivo arquivo, string path)
	{
		string json = JsonConvert.SerializeObject(arquivo, Formatting.Indented);

		if (!File.Exists(path))
		{
			using (StreamWriter sw = File.CreateText(path))
			{
				sw.Write(json.Normalize());
				sw.Close();
			}
		}
	}

	static void DownloadImagem(Arquivo arquivo, string path)
	{
		if (!string.IsNullOrEmpty(arquivo.url_imagem))
		{
			string url = arquivo.url_imagem;

			try
			{
				using (WebClient client = new WebClient())
				{
					if (!url.Equals("//f.i.uol.com.br/hunting/guia/3/furniture/images/guia-facebook-share.png"))
						client.DownloadFile(new Uri(url), path);
				}
			}
			catch
			{
			}
		}
	}

	static int RandomNumber()
	{
		Random random = new Random();
		return random.Next(1, 500000);
	}

	private static Arquivo RetornaParametros(string page)
	{
		try
		{
			string responseData = "";
			Arquivo link = new Arquivo(HttpUtility.HtmlDecode(ExtractParam("data-sharer-url=\"https://", "\"", page)));
			responseData = AcessarUrl("http://" + link.url_interna);

			Arquivo arquivo = new Arquivo(HttpUtility.HtmlDecode(ExtractParam("<title>", "-", responseData)),                                                                     //título
										  HttpUtility.HtmlDecode(ExtractParam("/restaurantes/", "/", responseData)),                                                              //Sub-categoria
										  HttpUtility.HtmlDecode(ExtractParam("<td>Horários</td>\n        <td>\n", "<svg", responseData)),                                        //Horário
										  "",                                                                                                                                     //Cotação toafim
										  HttpUtility.HtmlDecode(ExtractParam("<div class=\"star-rating star-rating--", "\">", responseData)),                                    //Cotação usuário
										  HttpUtility.HtmlDecode(ExtractParam("<td>Preço</td>\n      <td>\n", " </td>", responseData)),                                           //Faixa de preço
										  HttpUtility.HtmlDecode(RetornaCaracteristica(ExtractParam("<ul class=\"characteristics__list\">", " </ul>", responseData), "wifi")),    //Wifi
										  "",                                                                                                                                     //Valet
										  "",                                                                                                                                     //Delivery
										  HttpUtility.HtmlDecode(RetornaCaracteristica(ExtractParam("<ul class=\"characteristics__list\">", " </ul>", responseData), "reserva")), //Reserva
										  "",                                                                                                                                     //Descrição
										  HttpUtility.HtmlDecode(ExtractParam("<meta name=\"description\" content=\"", "\">", responseData)),                                     //Matéria
										  HttpUtility.HtmlDecode(ExtractParam("</svg>", "<", ExtractParam("<div class=\"card__footer\">", "/div>", page))),                       //Endereço/bairro
										  "",                                                                                                                                     //Site
										  HttpUtility.HtmlDecode(ExtractParam("<span class=\"js-map-address\" data-value=\"", "\">", responseData)),                        //Mapa
										  HttpUtility.HtmlDecode(ExtractParam("<meta property=\"og:image\" content=\"", "\">", responseData)));                                   //Imagem
			return arquivo;
		}
		catch
		{
			return null;
		}
	}

	private static string ExtractParam(string nameStartPosition, string nameEndPosition, string page)
	{
		string valor = "";

		int startDelimiter = page.IndexOf(nameStartPosition);

		int viewStateValuePosition = page.IndexOf(nameStartPosition, startDelimiter);

		int viewStateStartPosition = viewStateValuePosition + nameStartPosition.Length;
		int viewStateEndPosition = page.IndexOf(nameEndPosition, viewStateStartPosition);

		//Aqui vc vai obter o valor do campo
		valor = page.Substring(viewStateStartPosition, viewStateEndPosition - viewStateStartPosition);

		return valor;
	}

	private static string RetornaCaracteristica(string page, string caracteristica)
	{
		string valor = "Não";
		int startDelimiter = 0;

		if (caracteristica.Equals("wifi"))
		{
			startDelimiter = page.IndexOf("Tem wifi");
		}
		else if (caracteristica.Equals("reserva"))
		{
			startDelimiter = page.IndexOf("Aceita reserva por telefone");
		}

		if (startDelimiter != -1)
		{
			valor = "Sim";
		}

		return valor;
	}

	static string AcessarUrl(string url)
	{
		CookieContainer cookies = new CookieContainer();
		HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
		webRequest.CookieContainer = cookies;
		webRequest.Method = "GET";
		webRequest.AllowAutoRedirect = true;
		webRequest.MaximumAutomaticRedirections = 200;
		webRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.6 (KHTML, like Gecko) Chrome/16.0.897.0 Safari/535.6";
		StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
		string responseData = responseReader.ReadToEnd();
		responseReader.Close();
		//System.Threading.Thread.Sleep(100);
		return responseData;
	}
}