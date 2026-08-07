using System.Net.Http.Json;
using PoslovnaLogika;

namespace PoslovnaLogika.Servisi
{
    public class ParametriPoslovnihPravilaServis
    {
        private readonly HttpClient _httpClient;

        public ParametriPoslovnihPravilaServis(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ParametriPoslovnihPravila>
            DajParametreAsync()
        {
            ParametriPoslovnihPravila? parametri =
                await _httpClient.GetFromJsonAsync
                <ParametriPoslovnihPravila>(
                    "api/ParametriPoslovnihPravila"
                );

            if (parametri == null)
            {
                throw new InvalidOperationException(
                    "REST API nije vratio parametre poslovnih pravila."
                );
            }

            return parametri;
        }
    }
}