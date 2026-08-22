using RVS_Aplikacija.ViewModels;

namespace RVS_Aplikacija.Servisi.Pdf
{
	public interface IPdfUgovorServis
	{
		byte[] GenerisiPdf(UgovorViewModel ugovor);
	}
}
