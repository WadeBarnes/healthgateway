//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
namespace HealthGateway.Common.Delegates
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using HealthGateway.Common.Constants.PHSA;
    using HealthGateway.Common.Delegates;
    using HealthGateway.Common.Models;
    using HealthGateway.Common.Models.PHSA;
    using HealthGateway.Common.Utils;
    using Microsoft.Extensions.Configuration;

    /// <inheritdoc/>
    public class ReportDelegate : IReportDelegate
    {
        private const string BorderDashed = "dashed";
        private const string BorderSolid = "solid";
        private readonly IIronPDFDelegate ironPdfDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportDelegate"/> class.
        /// </summary>
        /// <param name="ironPdfDelegate">The injected iron pdf delegate.</param>
        public ReportDelegate(IIronPDFDelegate ironPdfDelegate)
        {
            this.ironPdfDelegate = ironPdfDelegate;
        }

        /// <inheritdoc/>
        public RequestResult<ReportModel> GetVaccineStatusPDF(VaccineStatus vaccineStatus, Address? address)
        {
            IronPDFRequestModel pdfRequest = new ();
            pdfRequest.FileName = "BCVaccineCard";
            pdfRequest.Data.Add("bcTopLogoImageSrc", AssetReader.Read("HealthGateway.Common.Assets.Images.BCID_V_rgb_pos.png", true));
            pdfRequest.Data.Add("bcLogoImageSrc", AssetReader.Read("HealthGateway.Common.Assets.Images.BCID_H_rgb_pos.png", true));
            pdfRequest.Data.Add("currentDate", DateTime.Now.ToLongDateString());
            pdfRequest.HtmlTemplate = AssetReader.Read("HealthGateway.Common.Assets.Templates.VaccineStatusCard.html") !;

            pdfRequest.Data.Add("birthdate", vaccineStatus.Birthdate!.Value.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture).ToUpper(CultureInfo.InvariantCulture));
            pdfRequest.Data.Add("name", $"{vaccineStatus.FirstName} {vaccineStatus.LastName}");
            pdfRequest.Data.Add("qrCodeImageSrc", vaccineStatus.QRCode.Data);

            pdfRequest.Data.Add("addressee", address == null ? string.Empty : $"{vaccineStatus.FirstName} {vaccineStatus.LastName}");
            pdfRequest.Data.Add("street", address == null ? string.Empty : string.Join("<br />", address.StreetLines));
            pdfRequest.Data.Add("city", address?.City);
            pdfRequest.Data.Add("provinceOrState", address?.State);
            pdfRequest.Data.Add("code", address?.PostalCode);
            pdfRequest.Data.Add("country", address?.Country);

            switch (vaccineStatus.State)
            {
                case VaccineState.AllDosesReceived:
                    pdfRequest.Data.Add("resultText", "Vaccinated");
                    pdfRequest.Data.Add("resultBorder", BorderSolid);
                    string? checkMarkBase64 = AssetReader.Read("HealthGateway.Common.Assets.Images.checkmark-black.svg", true);
                    pdfRequest.Data.Add("resultImageSrc", $"data:image/svg+xml;base64, {checkMarkBase64}");
                    pdfRequest.Data.Add("resultImageDisplay", "block");
                    break;
                case VaccineState.PartialDosesReceived:
                    pdfRequest.Data.Add("resultText", "Partially Vaccinated");
                    pdfRequest.Data.Add("resultBorder", BorderDashed);
                    pdfRequest.Data.Add("resultImageSrc", string.Empty);
                    pdfRequest.Data.Add("resultImageDisplay", "none");
                    break;
                case VaccineState.Exempt:
                    pdfRequest.Data.Add("resultText", "Exempt");
                    pdfRequest.Data.Add("resultBorder", BorderDashed);
                    pdfRequest.Data.Add("resultImageSrc", string.Empty);
                    pdfRequest.Data.Add("resultImageDisplay", "none");
                    break;
                default:
                    pdfRequest.Data.Add("resultText", "No Records Found");
                    pdfRequest.Data.Add("resultBorder", BorderDashed);
                    pdfRequest.Data.Add("resultImageSrc", string.Empty);
                    pdfRequest.Data.Add("resultImageDisplay", "none");
                    break;
            }

            return this.ironPdfDelegate.Generate(pdfRequest);
        }

        /// <inheritdoc/>
        public RequestResult<ReportModel> GetVaccineStatusAndRecordPDF(VaccineStatus vaccineStatus, Address? address, string base64RecordCard)
        {
            RequestResult<ReportModel> vaccineStatusResult = this.GetVaccineStatusPDF(vaccineStatus, address);
            return this.ironPdfDelegate.Merge(vaccineStatusResult.ResourcePayload!.Data, base64RecordCard, vaccineStatusResult.ResourcePayload.FileName);
        }
    }
}