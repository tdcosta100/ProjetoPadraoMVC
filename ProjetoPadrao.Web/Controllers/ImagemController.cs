using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjetoPadrao.Web.Controllers
{
    public class ImagemController : Controller
    {
        private static object _syncRoot = new object();

        public ActionResult Index(string nome)
        {
            var caminhoFisicoCache = Server.MapPath(string.Format("~/Images/Cache/{0}", nome));

            if (System.IO.File.Exists(caminhoFisicoCache))
            {
                System.IO.File.SetLastAccessTime(caminhoFisicoCache, DateTime.Now);
                return File(caminhoFisicoCache, MimeMapping.GetMimeMapping(caminhoFisicoCache));
            }

            var patternFilename = @"^(?<filename>.+?)(_(?<parameter>(?<name>w|h|x|y|ws|hs)(?<value>\d+)|(?<name>((?<original>original)|(?<resizeType>fit|crop))(?<value>)|(?<resizeType>pad)(?<padColor>[a-f0-9]{3}|[a-f0-9]{6})?(?<value>))))*$";
            var filename = System.IO.Path.GetFileNameWithoutExtension(nome);
            var extension = System.IO.Path.GetExtension(nome).ToLower();

            var matchFilename = System.Text.RegularExpressions.Regex.Match(filename, patternFilename);
            var parametros = new Dictionary<string, int>();

            for (int i = 0; i < matchFilename.Groups["parameter"].Captures.Count; i++)
            {
                var name = matchFilename.Groups["name"].Captures[i].Value;
                var value = matchFilename.Groups["value"].Captures[i].Value;

                if (!parametros.ContainsKey(name) && value != string.Empty)
                {
                    parametros.Add(name, int.Parse(value));
                }
            }

            bool original = matchFilename.Groups["original"].Captures.Count > 0;
            string tipoRedimensionamento = (matchFilename.Groups["resizeType"].Value != string.Empty) ? matchFilename.Groups["resizeType"].Value : "fit";

            var caminhoFisico = Server.MapPath(string.Format("~/Images/Upload/{0}{1}{2}", (System.IO.Path.GetDirectoryName(nome) != string.Empty) ? string.Format("{0}/", System.IO.Path.GetDirectoryName(nome)) : string.Empty, matchFilename.Groups["filename"].Value, extension));

            if (System.IO.File.Exists(caminhoFisico))
            {
                if (original)
                {
                    return File(caminhoFisico, MimeMapping.GetMimeMapping(caminhoFisico));
                }

                try
                {
                    lock (_syncRoot)
                    {
                        using (var arquivoImagemOriginal = System.IO.File.OpenRead(caminhoFisico))
                        {
                            var imagemOriginal = BitmapDecoder.Create(arquivoImagemOriginal, BitmapCreateOptions.None, BitmapCacheOption.None).Frames.FirstOrDefault();

                            Rect? retanguloOrigem = null;

                            if (parametros.ContainsKey("x") && parametros.ContainsKey("y") && parametros.ContainsKey("ws") && parametros.ContainsKey("hs"))
                            {
                                retanguloOrigem = new Rect(new Point(parametros["x"], parametros["y"]), new Size(parametros["ws"], parametros["hs"]));
                            }

                            Size? tamanhoDestino = null;

                            if (parametros.ContainsKey("w") || parametros.ContainsKey("h"))
                            {
                                tamanhoDestino = new Size(parametros.ContainsKey("w") ? parametros["w"] : 0, parametros.ContainsKey("h") ? parametros["h"] : 0);
                            }

                            var imagem = Redimensionar(imagemOriginal, retanguloOrigem, tamanhoDestino, false, tipoRedimensionamento, matchFilename.Groups["padColor"].Value != string.Empty ? (Color?)ColorConverter.ConvertFromString(string.Format("#{0}", matchFilename.Groups["padColor"].Value)) : null);

                            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(caminhoFisicoCache)))
                            {
                                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(caminhoFisicoCache));
                            }

                            if (imagem == imagemOriginal)
                            {
                                System.IO.File.Copy(caminhoFisico, caminhoFisicoCache);
                            }
                            else
                            {
                                using (var arquivoImagem = System.IO.File.Create(caminhoFisicoCache))
                                {
                                    BitmapEncoder encoder = null;

                                    switch (extension)
                                    {
                                        case ".png":
                                            encoder = new PngBitmapEncoder();
                                            break;
                                        case ".jpg":
                                            encoder = new JpegBitmapEncoder();
                                            ((JpegBitmapEncoder)encoder).QualityLevel = 100;
                                            break;
                                        default:
                                            throw new InvalidOperationException("Tipo de imagem não suportado");
                                    }

                                    encoder.Frames.Add(imagem);
                                    encoder.Save(arquivoImagem);
                                }
                            }
                        }

                        return File(caminhoFisicoCache, MimeMapping.GetMimeMapping(caminhoFisico));
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return HttpNotFound();
        }

        /// <summary>
        /// Redimensiona uma imagem, de acordo com os parâmetros fornecidos
        /// </summary>
        /// <param name="imagemOriginal">Imagem a ser redimensionada</param>
        /// <param name="retanguloOrigem">Porção da imagem a ser utilizada para o redimensionamento (opcional)</param>
        /// <param name="tamanhoDestino">Tamanho da imagem redimensionada (2048x2048, caso o parâmetro seja nulo)</param>
        /// <param name="expandir">Expandir a imagem caso o tamanho da imagem original seja menor que o valor do parâmetro tamanhoDestino</param>
        /// <param name="tipoRedimensionamento">Tipo de redimensionamento (fit, crop ou pad)</param>
        /// <param name="corPreenchimento">Cor do preenchimento, caso o parâmetro tipoRedimensionamento seja "pad"</param>
        /// <returns>Imagem redimensionada</returns>
        private BitmapFrame Redimensionar(BitmapFrame imagemOriginal, Rect? retanguloOrigem, Size? tamanhoDestino, bool expandir, string tipoRedimensionamento = "fit", Color? corPreenchimento = null)
        {
            Rect retanguloA = new Rect(), retanguloB = new Rect(), retanguloDestino = new Rect();
            double dimensao1Original = 0, dimensao1 = 0, dimensao2 = 0, dimensao3 = 0;
            string tipoDimensao1 = null, tipoDimensao2 = null, tipoDimensao3 = null;
            double proporcao = 0, proporcaoRetanguloA = 0, proporcaoRetanguloB = 0, proporcaoOriginal = 0;

            if (!retanguloOrigem.HasValue)
            {
                retanguloOrigem = new Rect(0, 0, imagemOriginal.PixelWidth, imagemOriginal.PixelHeight);
            }

            proporcaoOriginal = retanguloOrigem.Value.Width / retanguloOrigem.Value.Height;

            if (!tamanhoDestino.HasValue)
            {
                tamanhoDestino = new Size(Math.Min(retanguloOrigem.Value.Width, 2048), Math.Min(retanguloOrigem.Value.Height, 2048));
            }
            else if (tamanhoDestino.Value.Height == 0)
            {
                tamanhoDestino = new Size(tamanhoDestino.Value.Width, Math.Floor(tamanhoDestino.Value.Width / proporcaoOriginal));
            }
            else if (tamanhoDestino.Value.Width == 0)
            {
                tamanhoDestino = new Size(Math.Floor(tamanhoDestino.Value.Height * proporcaoOriginal), tamanhoDestino.Value.Height);
            }

            if (!expandir && tamanhoDestino.HasValue)
            {
                tamanhoDestino = new Size(Math.Min(tamanhoDestino.Value.Width, retanguloOrigem.Value.Width), Math.Min(tamanhoDestino.Value.Height, retanguloOrigem.Value.Height));
            }

            if (
                retanguloOrigem.Value.X == 0
                && retanguloOrigem.Value.Y == 0
                && retanguloOrigem.Value.Width == imagemOriginal.PixelWidth
                && retanguloOrigem.Value.Height == imagemOriginal.PixelHeight
                && retanguloOrigem.Value.Size.Equals(tamanhoDestino)
            )
            {
                return imagemOriginal;
            }

            switch (tipoRedimensionamento)
            {
                case "crop":
                    retanguloB = new Rect(new Point(0, 0), retanguloOrigem.Value.Size);
                    retanguloA = new Rect(new Point(0, 0), tamanhoDestino.Value);
                    break;
                case "fit":
                case "pad":
                    retanguloA = new Rect(new Point(0, 0), retanguloOrigem.Value.Size);
                    retanguloB = new Rect(new Point(0, 0), tamanhoDestino.Value);
                    break;
                default:
                    throw new ArgumentException("Tipo de redimensionamento inválido", "tipoRedimensionamento");
            }

            proporcaoRetanguloA = retanguloA.Width / retanguloA.Height;
            proporcaoRetanguloB = retanguloB.Width / retanguloB.Height;

            if (proporcaoRetanguloA != proporcaoRetanguloB)
            {
                if (proporcaoRetanguloA > proporcaoRetanguloB)
                {
                    tipoDimensao1 = "Height";
                    tipoDimensao2 = "Width";
                    tipoDimensao3 = "Y";

                    proporcao = 1 / proporcaoRetanguloA;
                }
                else if (proporcaoRetanguloA < proporcaoRetanguloB)
                {
                    tipoDimensao1 = "Width";
                    tipoDimensao2 = "Height";
                    tipoDimensao3 = "X";

                    proporcao = proporcaoRetanguloA;
                }

                dimensao1Original = dimensao1 = (double)retanguloB.GetType().GetProperty(tipoDimensao1).GetValue(retanguloB);
                dimensao2 = (double)retanguloB.GetType().GetProperty(tipoDimensao2).GetValue(retanguloB);

                dimensao1 = Math.Floor(dimensao2 * proporcao);
                dimensao3 = Math.Round((dimensao1Original - dimensao1) / 2.0);

                object retangulo = retanguloB;

                retanguloB.GetType().GetProperty(tipoDimensao1).SetValue(retangulo, dimensao1);
                retanguloB.GetType().GetProperty(tipoDimensao3).SetValue(retangulo, dimensao3);

                retanguloB = (Rect)retangulo;
            }

            switch (tipoRedimensionamento)
            {
                case "crop":
                    retanguloOrigem = retanguloB;
                    retanguloDestino = retanguloA;
                    break;
                case "fit":
                case "pad":
                    retanguloOrigem = retanguloA;
                    retanguloDestino = retanguloB;

                    if (tipoRedimensionamento == "fit")
                    {
                        tamanhoDestino = retanguloDestino.Size;
                        retanguloDestino.Location = new Point(0, 0);
                    }
                    break;
                default:
                    break;
            }

            var drawingGroup = new DrawingGroup();

            RenderOptions.SetBitmapScalingMode(drawingGroup, BitmapScalingMode.Fant);

            var imagemCortada = new CroppedBitmap(imagemOriginal, new Int32Rect(Convert.ToInt32(retanguloOrigem.Value.X), Convert.ToInt32(retanguloOrigem.Value.Y), Convert.ToInt32(retanguloOrigem.Value.Width), Convert.ToInt32(retanguloOrigem.Value.Height)));

            drawingGroup.Children.Add(new ImageDrawing(imagemCortada, retanguloDestino));

            var drawingVisual = new DrawingVisual();

            var drawingContext = drawingVisual.RenderOpen();

            if (tipoRedimensionamento == "pad")
            {
                drawingContext.DrawRectangle(new SolidColorBrush(corPreenchimento ?? Colors.White), null, new Rect(new Point(0, 0), tamanhoDestino.Value));
            }

            drawingContext.DrawDrawing(drawingGroup);

            var renderTargetBitmap = new RenderTargetBitmap(Convert.ToInt32(tamanhoDestino.Value.Width), Convert.ToInt32(tamanhoDestino.Value.Height), 96, 96, PixelFormats.Default);

            drawingContext.Close();

            renderTargetBitmap.Render(drawingVisual);

            if (imagemOriginal.ColorContexts != null)
            {
                return BitmapFrame.Create(new ColorConvertedBitmap(renderTargetBitmap, imagemOriginal.ColorContexts[0], new ColorContext(PixelFormats.Bgra32), PixelFormats.Pbgra32));
            }

            return BitmapFrame.Create(renderTargetBitmap);
        }

        public static void LimparCache()
        {
            var dataLimite = DateTime.Now.AddMonths(-1);

            var caminhoCache = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/Cache");

            var caminhoArquivos = System.IO.Directory.EnumerateFiles(caminhoCache, "*.jpg", System.IO.SearchOption.AllDirectories).Union(System.IO.Directory.EnumerateFiles(caminhoCache, "*.jpeg", System.IO.SearchOption.AllDirectories)).Union(System.IO.Directory.EnumerateFiles(caminhoCache, "*.png", System.IO.SearchOption.AllDirectories));

            foreach (var caminhoArquivo in caminhoArquivos)
            {
                var dataAcesso = System.IO.File.GetLastAccessTime(caminhoArquivo);

                if (dataAcesso < dataLimite)
                {
                    System.IO.File.Delete(caminhoArquivo);
                }
            }
        }
    }
}