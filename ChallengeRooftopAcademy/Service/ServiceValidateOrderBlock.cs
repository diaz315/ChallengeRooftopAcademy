using ChallengeRooftopAcademy.Models;
using ChallengeRooftopAcademy.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeRooftopAcademy.Service
{
    public class ServiceValidateOrderBlock : IServiceValidateOrderBlock
    {
        public readonly IHttpService _rooftopService;
        public readonly IServiceCache _serviceCache;

        public Dictionary<string, string> dictionaryCheck = new Dictionary<string, string>();

        public ServiceValidateOrderBlock(IHttpService rooftopService, IServiceCache serviceCache)
        {
            _rooftopService = rooftopService;
            _serviceCache = serviceCache;
        }

        public async Task<List<string>> check(List<string> blocksData, string token)
        {
            try
            {
                List<string> resultado = new List<string>();

                Dictionary<string, string> dictionaryCheck = new Dictionary<string, string>();

                dictionaryCheck = blocksData.ToDictionary(x => x);

                bool flagIteration = true;

                string flagBlockValue = blocksData[0];
                resultado.Add(flagBlockValue);
                dictionaryCheck.Remove(flagBlockValue);

                int counter = 0;

                while (flagIteration)
                {
                    foreach (var item in dictionaryCheck)
                    {
                        counter++;

                        //Como e el ultimo, no lo comparamos , ya que no hay mas con quien hacerlo y ahorramos una consulta del api
                        if (dictionaryCheck.Count == 1)
                        {
                            resultado.Add(item.Value);
                            flagIteration = false;
                            break;
                        }

                        var responseCheck = new ResponseCheck
                        {
                            blocks = new List<string> { flagBlockValue, item.Value },
                            merged = flagBlockValue + item.Value
                        };

                        var result = await _rooftopService.getCheck(responseCheck);

                        if (counter>0 && !result.consultedToApi) {
                            counter -= 1;
                        }

                        if (result != null && result.message)
                        {
                            resultado.Add(item.Value);
                            flagBlockValue = item.Value;
                            dictionaryCheck.Remove(item.Key);
                            break;
                        }
                        //Codigo de seguridad, para el maximo de permutaciones permitidas
                        if (counter == ((blocksData.Count) * (blocksData.Count)))
                        {
                            Console.WriteLine("No se ha encontrado coincidencias");
                            return new List<string>();
                        };
                    }
                }

                //Concatenando lista de valores
                string combinedString = string.Join("", resultado);

                var finnalCompare = await _rooftopService.getCheck(new ResponseCheck
                {
                    encoded = combinedString,
                });

                finnalCompare ??= new ResponseCheck();

                var message = finnalCompare.message ? "Correcto" : "Incorrecto";

                if (!finnalCompare.message)
                    return new List<string>();

                Console.WriteLine($"Resultado: {message}");
                Console.WriteLine($"Total Api request: {counter}");

                return resultado;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<List<string>> executeCheck(string email)
        {
            try
            {
                //Validando formato email
                new MailAddress(email);
                //get token
                await _rooftopService.getToken(email);

                //get blocks
                var blocks = await _rooftopService.getBlocks();

                //get token in cache to send
                var responseTokenCache = _serviceCache.get<ResponseToken>("token");

                if (blocks.data.Any())
                {
                    var blocksList = await check(blocks.data, responseTokenCache.token);
                    int counter = 1;
                    Console.WriteLine("");
                    Console.WriteLine("Imprimiendo resultados:");
                    Console.WriteLine("");
                    foreach (var block in blocksList)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(counter++);
                        sb.Append(" " + block.ToString());

                        Console.WriteLine(sb);
                    }

                    return blocksList;
                }
                return new List<string>();

            }
            catch (Exception ex) {
                throw ex;
            }            
        }
    }
}
