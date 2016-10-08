using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPadrao.Web.Controllers
{
    public class ImagemController : Controller
    {
        public ActionResult Index(string nome)
        {
            var caminhoFisicoCache = Server.MapPath(string.Format("~/Images/Cache/{0}", nome));

            if (System.IO.File.Exists(caminhoFisicoCache))
            {
                return File(caminhoFisicoCache, MimeMapping.GetMimeMapping(caminhoFisicoCache));
            }

            var patternFilename = @"^(?<filename>.+?)(_(?<parameter>(?<name>w|h|x|y|ws|hs)(?<value>\d+)|(?<name>((?<original>original)|(?<resizeType>fit|crop|pad))(?<value>))))*$";
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

            var caminhoFisico = Server.MapPath(string.Format("~/Images/Upload/{0}/{1}{2}", System.IO.Path.GetDirectoryName(nome), matchFilename.Groups["filename"].Value, extension));

            if (System.IO.File.Exists(caminhoFisico))
            {
                if (original)
                {
                    return File(caminhoFisico, MimeMapping.GetMimeMapping(caminhoFisico));
                }

                try
                {
                    using (var arquivoImagemOriginal = System.IO.File.OpenRead(caminhoFisico))
                    {
                        var imagemOriginal = Image.FromStream(arquivoImagemOriginal);
                        Rectangle? retanguloOrigem = null;

                        if (parametros.ContainsKey("x") && parametros.ContainsKey("y") && parametros.ContainsKey("ws") && parametros.ContainsKey("hs"))
                        {
                            retanguloOrigem = new Rectangle(new Point(parametros["x"], parametros["y"]), new Size(parametros["ws"], parametros["hs"]));
                        }

                        Size? tamanhoDestino = null;

                        if (parametros.ContainsKey("w") || parametros.ContainsKey("h"))
                        {
                            tamanhoDestino = new Size(parametros.ContainsKey("w") ? parametros["w"] : 0, parametros.ContainsKey("h") ? parametros["h"] : 0);
                        }

                        var imagem = Redimensionar(imagemOriginal, retanguloOrigem, tamanhoDestino, tipoRedimensionamento);

                        if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(caminhoFisicoCache)))
                        {
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(caminhoFisicoCache));
                        }

                        using (var arquivoImagem = System.IO.File.Create(caminhoFisicoCache))
                        {
                            switch (extension)
                            {
                                case ".png":
                                    imagem.Save(arquivoImagem, ImageFormat.Png);
                                    break;
                                case ".jpg":
                                    imagem.Save(arquivoImagem, ImageCodecInfo.GetImageDecoders().Single(e => e.FormatID == ImageFormat.Jpeg.Guid), new EncoderParameters { Param = new[] { new EncoderParameter(Encoder.Quality, 100L) } });
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    return File(caminhoFisicoCache, MimeMapping.GetMimeMapping(caminhoFisico));
                }
                catch (Exception ex)
                {
                }
            }

            return HttpNotFound();
        }

        private Image Redimensionar(Image imagemOriginal, Rectangle? retanguloOrigem, Size? tamanhoDestino, string tipoRedimensionamento = "fit", Color? corPreenchimento = null)
        {
            var retanguloOrigemInterno = retanguloOrigem ?? new Rectangle(new Point(0, 0), new Size(imagemOriginal.Width, imagemOriginal.Height));
            var proporcaoOriginal = retanguloOrigemInterno.Width / (double)retanguloOrigemInterno.Height;
            var tamanhoDestinoInterno = tamanhoDestino ?? new Size(Math.Min(retanguloOrigemInterno.Width, 2048), Math.Min(retanguloOrigemInterno.Height, 2048));
            var retanguloDestinoInterno = new Rectangle(new Point(0, 0), tamanhoDestinoInterno);

            bool tamanhoFinalCalculado = false;

            if (retanguloDestinoInterno.Height == 0 || (!tamanhoDestino.HasValue && proporcaoOriginal > 1))
            {
                retanguloDestinoInterno.Height = Convert.ToInt32(Math.Floor(retanguloDestinoInterno.Width / proporcaoOriginal));
                tamanhoFinalCalculado = true;
            }
            else if (retanguloDestinoInterno.Width == 0 || (!tamanhoDestino.HasValue && proporcaoOriginal < 1))
            {
                retanguloDestinoInterno.Width = Convert.ToInt32(Math.Floor(retanguloDestinoInterno.Height * proporcaoOriginal));
                tamanhoFinalCalculado = true;
            }

            var proporcao = retanguloDestinoInterno.Width / (double)retanguloDestinoInterno.Height;

            switch (tipoRedimensionamento)
            {
                case "fit":
                    if (proporcaoOriginal > proporcao && !tamanhoFinalCalculado)
                    {
                        retanguloDestinoInterno.Height = Convert.ToInt32(Math.Floor(retanguloDestinoInterno.Width / proporcaoOriginal));
                    }
                    else if (proporcaoOriginal < proporcao && !tamanhoFinalCalculado)
                    {
                        retanguloDestinoInterno.Width = Convert.ToInt32(Math.Floor(retanguloDestinoInterno.Height * proporcaoOriginal));
                    }

                    tamanhoDestinoInterno = retanguloDestinoInterno.Size;
                    break;
                case "crop":
                    if (proporcao > proporcaoOriginal && !retanguloOrigem.HasValue)
                    {
                        var alturaOriginal = retanguloOrigemInterno.Height;
                        retanguloOrigemInterno.Height = Convert.ToInt32(Math.Floor(retanguloOrigemInterno.Width / proporcao));
                        retanguloOrigemInterno.Y = Convert.ToInt32(Math.Round((alturaOriginal - retanguloOrigemInterno.Height) / 2.0));
                    }
                    else if (proporcao < proporcaoOriginal && !retanguloOrigem.HasValue)
                    {
                        var larguraOriginal = retanguloOrigemInterno.Width;
                        retanguloOrigemInterno.Width = Convert.ToInt32(Math.Floor(retanguloOrigemInterno.Height * proporcao));
                        retanguloOrigemInterno.X = Convert.ToInt32(Math.Round((larguraOriginal - retanguloOrigemInterno.Width) / 2.0));
                    }

                    tamanhoDestinoInterno = retanguloDestinoInterno.Size;
                    break;
                case "pad":
                    corPreenchimento = corPreenchimento ?? Color.White;

                    if (proporcaoOriginal > proporcao)
                    {
                        var alturaOriginal = retanguloDestinoInterno.Height;
                        retanguloDestinoInterno.Height = Convert.ToInt32(Math.Floor(retanguloDestinoInterno.Width / proporcaoOriginal));
                        retanguloDestinoInterno.Y = Convert.ToInt32(Math.Round((alturaOriginal - retanguloDestinoInterno.Height) / 2.0));
                    }
                    else if (proporcaoOriginal < proporcao)
                    {
                        var larguraOriginal = retanguloDestinoInterno.Width;
                        retanguloDestinoInterno.Width = Convert.ToInt32(Math.Floor(retanguloDestinoInterno.Height * proporcaoOriginal));
                        retanguloDestinoInterno.X = Convert.ToInt32(Math.Round((larguraOriginal - retanguloDestinoInterno.Width) / 2.0));
                    }
                    break;
                default:
                    break;
            }

            var imagem = new Bitmap(tamanhoDestinoInterno.Width, tamanhoDestinoInterno.Height);

            var graphics = Graphics.FromImage(imagem);
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            if (tipoRedimensionamento == "pad")
            {
                graphics.Clear(corPreenchimento.Value);
            }

            using (ImageAttributes attributes = new ImageAttributes())
            {
                attributes.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);

                graphics.DrawImage(imagemOriginal, retanguloDestinoInterno, retanguloOrigemInterno.X, retanguloOrigemInterno.Y, retanguloOrigemInterno.Width, retanguloOrigemInterno.Height, GraphicsUnit.Pixel, attributes);
            }

            return imagem;
        }
    }
}