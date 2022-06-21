using ChallengeRooftopAcademy.Models;
using ChallengeRooftopAcademy.Service;
using ChallengeRooftopAcademy.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChallengeRooftopAcademy.UnitTest
{
    [TestClass]
    public class ServiceValidateOrderBlockTest
    {
        public Mock<IHttpService> mhttpService;
        public Mock<IServiceCache> mserviceCache;
        public ServiceValidateOrderBlock serviceValidateOrderBlock;
        public Blocks bloks { get; set; }


        [TestInitialize]
        public void Initialize()
        {
            mhttpService = new Mock<IHttpService>();
            mserviceCache = new Mock<IServiceCache>();
            bloks = new Blocks
            {
                data = new List<string>
                {
                    "JDn6x4ZG9u1llIkS2DL9PbcO7P0KxbI3FwVHaMywajsjKVc1f80JojYnhqLPT1pNhVd6zYpVrE8CUadNmCN6CN0ziTTedkxgjfrM",
                    "Y8uOLmtvhYkn2ozZXFyQiFFhK8ndEBmEQjN49E1XdpPQjM2LGecVHrpbKnrErXbrlskEWK5M0l3I6mL0VZCF8tcGmYNeWiVMJ3mU",
                    "nUlKiT6CxGMDt9nmdzJIXuLVNUZ2HR9GcEktrljLgwmyMyJ23KG4ItNvVUeyKygKMB2cW1w3B7oNna9d5tZnT5Ywy2HkQVdTNmid",
                    "zwcYPMhttdUDbJWbHknI1TBx6LnCOkFXeMtsjbVL43iSPUh8u2rGSq1q9hlnMlUJaSyjSkgG0pEoULzmH6ZS7vhkgsPOOxOz8tNg",
                    "fdGbAhn6wT6MI9XRmD4GrHqrcjhZhxV7U2wbyucu5AV6oIFBaPwDO3Y652dmK28sjMZvdHFfHu921VnuVHwW4fzqHiofjiRaF0Ig",
                    "VjUpkzMNuLZZPQyVsL7HFMn1Tgrk7iedN0q7fJK44NsXKZtyp2Rf1s1oI5hTxppJDl5PIKYQbiZ1ofQp40LJtK33HWN49ZYYKNta",
                    "29LNcbxU3KYuCVLuuYUdotO3Em342KoeCMtOMUEP36Xme8nVCaoLXi58aXPGA6XgddkEi112Om9szG1gtvUMG7hP5lA1nVNk52fW",
                    "AZ1pim7RCWAI5tPZEycVIcpM7ArSZw5ZPYH4zeWdqdO5Cx8mD7x4tpzQVeMx2sgaGEEy7WDYCWQjZ2JjwQdhgW8gWrbabvSD1XX3",
                    "NnBI2xZGjM6jTImgwHVkHTXQ8NPzE73u315b2dn97T56HGquIvOaL1DNSeOpGAtmCBkHxzz1JDvDwOpOt3WYwaAjmZYfAMDktLtV"
                },
                chunkSize = 100,
                length = 900
            };

            mhttpService.Setup(x => x.getToken(It.IsAny<string>())).Returns(Task.FromResult("efd78828-6c97-4e82-8653-ea4736979c76"));
            mhttpService.Setup(x => x.getBlocks()).Returns(Task.FromResult(bloks));

            mserviceCache.Setup(x => x.set("token", "efd78828-6c97-4e82-8653-ea4736979c76"));
            mserviceCache.Setup(x => x.get<ResponseToken>("token")).Returns(new ResponseToken { token = "efd78828-6c97-4e82-8653-ea4736979c76" });
        }

        private void LoadFakeApiCheck(string resultEncoded) {
            var resposecheckList = new List<ResponseCheck>
            {
                new ResponseCheck
                {
                    blocks = new List<string> { bloks.data[0], bloks.data[2] },
                    merged = bloks.data[0]+bloks.data[2]
                },
                new ResponseCheck
                {
                    blocks = new List<string> { bloks.data[2], bloks.data[4] },
                    merged = bloks.data[2]+bloks.data[4]
                },
                new ResponseCheck
                {
                    blocks = new List<string> { bloks.data[4], bloks.data[6] },
                    merged = bloks.data[4]+bloks.data[6]
                },
                new ResponseCheck
                {
                    blocks = new List<string> { bloks.data[6], bloks.data[1] },
                    merged = bloks.data[6]+bloks.data[1]

                },
                new ResponseCheck
                {
                    blocks = new List<string> { bloks.data[1], bloks.data[8] },
                    merged = bloks.data[2]+bloks.data[8]

                },
                new ResponseCheck
                {
                    blocks = new List<string> { bloks.data[8], bloks.data[5] },
                    merged = bloks.data[8]+bloks.data[5]

                },
                new ResponseCheck
                {
                    blocks = new List<string> { bloks.data[5], bloks.data[3] },
                    merged = bloks.data[5]+bloks.data[3]

                },
                new ResponseCheck
                {
                    blocks = new List<string> { bloks.data[3], bloks.data[7] },
                    merged = bloks.data[3]+bloks.data[7]

                }
            };

            var resposecheckBadList = new List<ResponseCheck>();

            for (int i = 0; i < 8; i++)
            {
                resposecheckBadList.Add(new ResponseCheck
                {
                    blocks = new List<string> { bloks.data[i], bloks.data[i + 1] },
                    merged = bloks.data[i]+bloks.data[i + 1]
                });
            }

            resposecheckList.ForEach(resposecheck =>
            {
                mhttpService.Setup(x => x.getCheck(It.Is<ResponseCheck>(x => x.merged.Equals(string.Join("", resposecheck.blocks))))).Returns(Task.FromResult(new ResponseCheck { message = true }));
            });

            resposecheckBadList.ForEach(resposecheck =>
            {
                mhttpService.Setup(x => x.getCheck(It.Is<ResponseCheck>(x => x.merged.Equals(string.Join("", resposecheck.blocks))))).Returns(Task.FromResult(new ResponseCheck { message = false }));
            });

            mhttpService.Setup(x => x.getCheck(It.Is<ResponseCheck>(x => x.encoded.Equals(resultEncoded)))).Returns(Task.FromResult(new ResponseCheck { message = true }));
        }


        [DataRow("moon@moon.com")]
        [DataTestMethod]
        public async Task ShoulBeReturnSuccess(string email)
        {
            mhttpService.Setup(x => x.getCheck(It.IsAny<ResponseCheck>())).Returns(Task.FromResult(new ResponseCheck { message = true }));
            serviceValidateOrderBlock = new ServiceValidateOrderBlock(mhttpService.Object, mserviceCache.Object);
            var result = await serviceValidateOrderBlock.executeCheck(email);
            mhttpService.Verify(x => x.getCheck(It.IsAny<ResponseCheck>()), Times.Exactly(bloks.data.Count - 1));
            Assert.AreEqual(result.Count, bloks.data.Count);
        }

        [DataRow("moon@moon.com")]
        [DataTestMethod]
        public async Task ShoulBeReturnZero(string email)
        {
            mhttpService.Setup(x => x.getCheck(It.IsAny<ResponseCheck>())).Returns(Task.FromResult(new ResponseCheck { message = false }));
            serviceValidateOrderBlock = new ServiceValidateOrderBlock(mhttpService.Object, mserviceCache.Object);
            var result = await serviceValidateOrderBlock.executeCheck(email);
            Assert.AreEqual(result.Count, 0);
        }

        [DataRow("moonmoon.com")]
        [DataTestMethod]
        public async Task ShoulBeReturnEmailError(string email)
        {
            string error = null;
            try
            {
                mhttpService.Setup(x => x.getCheck(It.IsAny<ResponseCheck>())).Returns(Task.FromResult(new ResponseCheck { message = false }));
                serviceValidateOrderBlock = new ServiceValidateOrderBlock(mhttpService.Object, mserviceCache.Object);
                var result = await serviceValidateOrderBlock.executeCheck(email);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            Console.WriteLine(error);
            Assert.IsNotNull(error);
        }

        [DataRow("moon@moon.com", "JDn6x4ZG9u1llIkS2DL9PbcO7P0KxbI3FwVHaMywajsjKVc1f80JojYnhqLPT1pNhVd6zYpVrE8CUadNmCN6CN0ziTTedkxgjfrMnUlKiT6CxGMDt9nmdzJIXuLVNUZ2HR9GcEktrljLgwmyMyJ23KG4ItNvVUeyKygKMB2cW1w3B7oNna9d5tZnT5Ywy2HkQVdTNmidfdGbAhn6wT6MI9XRmD4GrHqrcjhZhxV7U2wbyucu5AV6oIFBaPwDO3Y652dmK28sjMZvdHFfHu921VnuVHwW4fzqHiofjiRaF0Ig29LNcbxU3KYuCVLuuYUdotO3Em342KoeCMtOMUEP36Xme8nVCaoLXi58aXPGA6XgddkEi112Om9szG1gtvUMG7hP5lA1nVNk52fWY8uOLmtvhYkn2ozZXFyQiFFhK8ndEBmEQjN49E1XdpPQjM2LGecVHrpbKnrErXbrlskEWK5M0l3I6mL0VZCF8tcGmYNeWiVMJ3mUNnBI2xZGjM6jTImgwHVkHTXQ8NPzE73u315b2dn97T56HGquIvOaL1DNSeOpGAtmCBkHxzz1JDvDwOpOt3WYwaAjmZYfAMDktLtVVjUpkzMNuLZZPQyVsL7HFMn1Tgrk7iedN0q7fJK44NsXKZtyp2Rf1s1oI5hTxppJDl5PIKYQbiZ1ofQp40LJtK33HWN49ZYYKNtazwcYPMhttdUDbJWbHknI1TBx6LnCOkFXeMtsjbVL43iSPUh8u2rGSq1q9hlnMlUJaSyjSkgG0pEoULzmH6ZS7vhkgsPOOxOz8tNgAZ1pim7RCWAI5tPZEycVIcpM7ArSZw5ZPYH4zeWdqdO5Cx8mD7x4tpzQVeMx2sgaGEEy7WDYCWQjZ2JjwQdhgW8gWrbabvSD1XX3")]
        [DataTestMethod]
        public async Task ShoulBeReturnFinalCheckSuccess(string email,string resultEncoded)
        {
            LoadFakeApiCheck(resultEncoded);
            serviceValidateOrderBlock = new ServiceValidateOrderBlock(mhttpService.Object, mserviceCache.Object);
            var result = await serviceValidateOrderBlock.executeCheck(email);
            Assert.AreEqual(result.Count, bloks.data.Count);
        }

        [DataRow("moon@moon.com", "deberiafallareltest")]
        [DataTestMethod]
        public async Task ShoulBeReturnFinalCheckError(string email,string resultEncoded)
        {
            LoadFakeApiCheck(resultEncoded);
            serviceValidateOrderBlock = new ServiceValidateOrderBlock(mhttpService.Object, mserviceCache.Object);
            var result = await serviceValidateOrderBlock.executeCheck(email);
            Assert.AreEqual(result.Count, 0);
        }

    }
}
