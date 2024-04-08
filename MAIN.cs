using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssutaRequests
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Main
    {

        private MainHeader headerField;

        private MainBody bodyField;

        /// <remarks/>
        public MainHeader Header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                this.headerField = value;
            }
        }

        /// <remarks/>
        public MainBody Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MainHeader
    {

        private string caseFileField;

        private string requestNumField;

        private string xMLDateField;

        private string xMLHrField;

        private string reqNumLabField;

        private string sUPCodeField;

        private string instnameField;

        private string hosCodeField;

        private string unitCodeField;

        private string statusField;

        private string updateNumberField;

        private string updateReasonField;

        private string iDPatCodeField;

        private string patIDField;

        private string bdField;

        private string patLastNameField;

        private string patFirstNameField;

        private string sexField;

        private string pregField;

        private string pregWeekField;

        private string perioDateField;

        private string executeTimeField;

        private string relatedDocIDField;

        private string relatedDocNameField;

        private string docCountCodeField;

        private string docFaxNumField;

        private string execDocIDField;

        private string execDocNameField;

        private string refReasonField;

        private string[] pDFFilesField;

        /// <remarks/>
        public string CaseFile
        {
            get
            {
                return this.caseFileField;
            }
            set
            {
                this.caseFileField = value;
            }
        }

        /// <remarks/>
        public string RequestNum
        {
            get
            {
                return this.requestNumField;
            }
            set
            {
                this.requestNumField = value;
            }
        }

        /// <remarks/>
        public string XMLDate
        {
            get
            {
                return this.xMLDateField;
            }
            set
            {
                this.xMLDateField = value;
            }
        }

        /// <remarks/>
        public string XMLHr
        {
            get
            {
                return this.xMLHrField;
            }
            set
            {
                this.xMLHrField = value;
            }
        }

        /// <remarks/>
        public string ReqNumLab
        {
            get
            {
                return this.reqNumLabField;
            }
            set
            {
                this.reqNumLabField = value;
            }
        }

        /// <remarks/>
        public string SUPCode
        {
            get
            {
                return this.sUPCodeField;
            }
            set
            {
                this.sUPCodeField = value;
            }
        }

        /// <remarks/>
        public string Instname
        {
            get
            {
                return this.instnameField;
            }
            set
            {
                this.instnameField = value;
            }
        }

        /// <remarks/>
        public string HosCode
        {
            get
            {
                return this.hosCodeField;
            }
            set
            {
                this.hosCodeField = value;
            }
        }

        /// <remarks/>
        public string UnitCode
        {
            get
            {
                return this.unitCodeField;
            }
            set
            {
                this.unitCodeField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string UpdateNumber
        {
            get
            {
                return this.updateNumberField;
            }
            set
            {
                this.updateNumberField = value;
            }
        }

        /// <remarks/>
        public string UpdateReason
        {
            get
            {
                return this.updateReasonField;
            }
            set
            {
                this.updateReasonField = value;
            }
        }

        /// <remarks/>
        public string IDPatCode
        {
            get
            {
                return this.iDPatCodeField;
            }
            set
            {
                this.iDPatCodeField = value;
            }
        }

        /// <remarks/>
        public string PatID
        {
            get
            {
                return this.patIDField;
            }
            set
            {
                this.patIDField = value;
            }
        }

        /// <remarks/>
        public string BD
        {
            get
            {
                return this.bdField;
            }
            set
            {
                this.bdField = value;
            }
        }

        /// <remarks/>
        public string PatLastName
        {
            get
            {
                return this.patLastNameField;
            }
            set
            {
                this.patLastNameField = value;
            }
        }

        /// <remarks/>
        public string PatFirstName
        {
            get
            {
                return this.patFirstNameField;
            }
            set
            {
                this.patFirstNameField = value;
            }
        }

        /// <remarks/>
        public string Sex
        {
            get
            {
                return this.sexField;
            }
            set
            {
                this.sexField = value;
            }
        }

        /// <remarks/>
        public string Preg
        {
            get
            {
                return this.pregField;
            }
            set
            {
                this.pregField = value;
            }
        }

        /// <remarks/>
        public string PregWeek
        {
            get
            {
                return this.pregWeekField;
            }
            set
            {
                this.pregWeekField = value;
            }
        }

        /// <remarks/>
        public string PerioDate
        {
            get
            {
                return this.perioDateField;
            }
            set
            {
                this.perioDateField = value;
            }
        }

        /// <remarks/>
        public string ExecuteTime
        {
            get
            {
                return this.executeTimeField;
            }
            set
            {
                this.executeTimeField = value;
            }
        }

        /// <remarks/>
        public string RelatedDocID
        {
            get
            {
                return this.relatedDocIDField;
            }
            set
            {
                this.relatedDocIDField = value;
            }
        }

        /// <remarks/>
        public string RelatedDocName
        {
            get
            {
                return this.relatedDocNameField;
            }
            set
            {
                this.relatedDocNameField = value;
            }
        }

        /// <remarks/>
        public string DocCountCode
        {
            get
            {
                return this.docCountCodeField;
            }
            set
            {
                this.docCountCodeField = value;
            }
        }

        /// <remarks/>
        public string DocFaxNum
        {
            get
            {
                return this.docFaxNumField;
            }
            set
            {
                this.docFaxNumField = value;
            }
        }

        /// <remarks/>
        public string ExecDocID
        {
            get
            {
                return this.execDocIDField;
            }
            set
            {
                this.execDocIDField = value;
            }
        }

        /// <remarks/>
        public string ExecDocName
        {
            get
            {
                return this.execDocNameField;
            }
            set
            {
                this.execDocNameField = value;
            }
        }

        /// <remarks/>
        public string RefReason
        {
            get
            {
                return this.refReasonField;
            }
            set
            {
                this.refReasonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("FileName", IsNullable = false)]
        public string[] PDFFiles
        {
            get
            {
                return this.pDFFilesField;
            }
            set
            {
                this.pDFFilesField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MainBody
    {

        private string testTypeCodeField;

        private MainBodyTest[] testsField;

        private MainBodyLabTests labTestsField;

        private string txtLabTestResultField;

        private MainBodyPathology pathologyField;

        private string diagnosePatField;

        private string diagnosePatCodeField;

        private string sNOMEDPat1Field;

        private string sNOMEDPat2Field;

        private string patYNField;

        private string diagnosticTimeField;

        private string aNSDocIDField;

        private string aNSDocNameField;

        private MainBodyExtParameters extParametersField;

        /// <remarks/>
        public string TestTypeCode
        {
            get
            {
                return this.testTypeCodeField;
            }
            set
            {
                this.testTypeCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Test", IsNullable = false)]
        public MainBodyTest[] Tests
        {
            get
            {
                return this.testsField;
            }
            set
            {
                this.testsField = value;
            }
        }

        /// <remarks/>
        public MainBodyLabTests LabTests
        {
            get
            {
                return this.labTestsField;
            }
            set
            {
                this.labTestsField = value;
            }
        }

        /// <remarks/>
        public string TxtLabTestResult
        {
            get
            {
                return this.txtLabTestResultField;
            }
            set
            {
                this.txtLabTestResultField = value;
            }
        }

        /// <remarks/>
        public MainBodyPathology Pathology
        {
            get
            {
                return this.pathologyField;
            }
            set
            {
                this.pathologyField = value;
            }
        }

        /// <remarks/>
        public string DiagnosePat
        {
            get
            {
                return this.diagnosePatField;
            }
            set
            {
                this.diagnosePatField = value;
            }
        }

        /// <remarks/>
        public string DiagnosePatCode
        {
            get
            {
                return this.diagnosePatCodeField;
            }
            set
            {
                this.diagnosePatCodeField = value;
            }
        }

        /// <remarks/>
        public string SNOMEDPat1
        {
            get
            {
                return this.sNOMEDPat1Field;
            }
            set
            {
                this.sNOMEDPat1Field = value;
            }
        }

        /// <remarks/>
        public string SNOMEDPat2
        {
            get
            {
                return this.sNOMEDPat2Field;
            }
            set
            {
                this.sNOMEDPat2Field = value;
            }
        }

        /// <remarks/>
        public string PatYN
        {
            get
            {
                return this.patYNField;
            }
            set
            {
                this.patYNField = value;
            }
        }

        /// <remarks/>
        public string DiagnosticTime
        {
            get
            {
                return this.diagnosticTimeField;
            }
            set
            {
                this.diagnosticTimeField = value;
            }
        }

        /// <remarks/>
        public string ANSDocID
        {
            get
            {
                return this.aNSDocIDField;
            }
            set
            {
                this.aNSDocIDField = value;
            }
        }

        /// <remarks/>
        public string ANSDocName
        {
            get
            {
                return this.aNSDocNameField;
            }
            set
            {
                this.aNSDocNameField = value;
            }
        }

        /// <remarks/>
        public MainBodyExtParameters ExtParameters
        {
            get
            {
                return this.extParametersField;
            }
            set
            {
                this.extParametersField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MainBodyTest
    {

        private string testCodeField;

        private string sampleBarCodeField;

        private string numOfSamplesField;

        private string urgencyPatField;

        private string aNSTYPEField;

        /// <remarks/>
        public string TestCode
        {
            get
            {
                return this.testCodeField;
            }
            set
            {
                this.testCodeField = value;
            }
        }

        /// <remarks/>
        public string SampleBarCode
        {
            get
            {
                return this.sampleBarCodeField;
            }
            set
            {
                this.sampleBarCodeField = value;
            }
        }

        /// <remarks/>
        public string NumOfSamples
        {
            get
            {
                return this.numOfSamplesField;
            }
            set
            {
                this.numOfSamplesField = value;
            }
        }

        /// <remarks/>
        public string UrgencyPat
        {
            get
            {
                return this.urgencyPatField;
            }
            set
            {
                this.urgencyPatField = value;
            }
        }

        /// <remarks/>
        public string ANSTYPE
        {
            get
            {
                return this.aNSTYPEField;
            }
            set
            {
                this.aNSTYPEField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MainBodyLabTests
    {

        private MainBodyLabTestsTest testField;

        /// <remarks/>
        public MainBodyLabTestsTest Test
        {
            get
            {
                return this.testField;
            }
            set
            {
                this.testField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MainBodyLabTestsTest
    {

        private string labTestResultField;

        private string labTestUnitField;

        private string lowNormLabTestField;

        private string highNormLabTestField;

        private string codedResultField;

        /// <remarks/>
        public string LabTestResult
        {
            get
            {
                return this.labTestResultField;
            }
            set
            {
                this.labTestResultField = value;
            }
        }

        /// <remarks/>
        public string LabTestUnit
        {
            get
            {
                return this.labTestUnitField;
            }
            set
            {
                this.labTestUnitField = value;
            }
        }

        /// <remarks/>
        public string LowNormLabTest
        {
            get
            {
                return this.lowNormLabTestField;
            }
            set
            {
                this.lowNormLabTestField = value;
            }
        }

        /// <remarks/>
        public string HighNormLabTest
        {
            get
            {
                return this.highNormLabTestField;
            }
            set
            {
                this.highNormLabTestField = value;
            }
        }

        /// <remarks/>
        public string CodedResult
        {
            get
            {
                return this.codedResultField;
            }
            set
            {
                this.codedResultField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MainBodyPathology
    {

        private MainBodyPathologyTest testField;

        /// <remarks/>
        public MainBodyPathologyTest Test
        {
            get
            {
                return this.testField;
            }
            set
            {
                this.testField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MainBodyPathologyTest
    {

        private string organNamePatField;

        private string organLocPatField;

        private string organSidePatField;

        private string containerTYPField;

        private string biopsNumPatField;

        private string hystSizeField;

        private string fROZENYNField;

        private string slideNumPatField;

        private string procedurePatField;

        private string patDetailField;

        /// <remarks/>
        public string OrganNamePat
        {
            get
            {
                return this.organNamePatField;
            }
            set
            {
                this.organNamePatField = value;
            }
        }

        /// <remarks/>
        public string OrganLocPat
        {
            get
            {
                return this.organLocPatField;
            }
            set
            {
                this.organLocPatField = value;
            }
        }

        /// <remarks/>
        public string OrganSidePat
        {
            get
            {
                return this.organSidePatField;
            }
            set
            {
                this.organSidePatField = value;
            }
        }

        /// <remarks/>
        public string ContainerTYP
        {
            get
            {
                return this.containerTYPField;
            }
            set
            {
                this.containerTYPField = value;
            }
        }

        /// <remarks/>
        public string BiopsNumPat
        {
            get
            {
                return this.biopsNumPatField;
            }
            set
            {
                this.biopsNumPatField = value;
            }
        }

        /// <remarks/>
        public string HystSize
        {
            get
            {
                return this.hystSizeField;
            }
            set
            {
                this.hystSizeField = value;
            }
        }

        /// <remarks/>
        public string FROZENYN
        {
            get
            {
                return this.fROZENYNField;
            }
            set
            {
                this.fROZENYNField = value;
            }
        }

        /// <remarks/>
        public string SlideNumPat
        {
            get
            {
                return this.slideNumPatField;
            }
            set
            {
                this.slideNumPatField = value;
            }
        }

        /// <remarks/>
        public string ProcedurePat
        {
            get
            {
                return this.procedurePatField;
            }
            set
            {
                this.procedurePatField = value;
            }
        }

        /// <remarks/>
        public string PatDetail
        {
            get
            {
                return this.patDetailField;
            }
            set
            {
                this.patDetailField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MainBodyExtParameters
    {

        private MainBodyExtParametersParams paramsField;

        /// <remarks/>
        public MainBodyExtParametersParams Params
        {
            get
            {
                return this.paramsField;
            }
            set
            {
                this.paramsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MainBodyExtParametersParams
    {

        private string parameterCodeField;

        private string hebTextField;

        private string engTextField;

        private string numericalField;

        /// <remarks/>
        public string ParameterCode
        {
            get
            {
                return this.parameterCodeField;
            }
            set
            {
                this.parameterCodeField = value;
            }
        }

        /// <remarks/>
        public string HebText
        {
            get
            {
                return this.hebTextField;
            }
            set
            {
                this.hebTextField = value;
            }
        }

        /// <remarks/>
        public string EngText
        {
            get
            {
                return this.engTextField;
            }
            set
            {
                this.engTextField = value;
            }
        }

        /// <remarks/>
        public string Numerical
        {
            get
            {
                return this.numericalField;
            }
            set
            {
                this.numericalField = value;
            }
        }
    }


}

