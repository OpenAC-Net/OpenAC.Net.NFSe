using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Windows.Forms;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.DANFSe.FastReport.OpenSource;
using OpenAC.Net.NFSe.DANFSe.QuestPdf;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Demo;

public partial class FormMain : Form, IOpenLog
{
    #region Fields

    private readonly OpenNFSe openNFSe;
    private readonly OpenConfig config;

    #endregion Fields

    #region Constructors

    public FormMain()
    {
        InitializeComponent();
        openNFSe = new OpenNFSe();
        config = OpenConfig.CreateOrLoad(Path.Combine(Application.StartupPath, "nfse.config"));
        dgvCidades.AutoGenerateColumns = false;
    }

    #endregion Constructors

    #region Methods

    #region EventHandlers

    private void btnEditCidade_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            var municipio = cmbCidades.GetSelectedValue<OpenMunicipioNFSe>();
            if (municipio == null) return;

            if (FormEdtMunicipio.Editar(municipio).Equals(DialogResult.Cancel)) return;

            LoadData();
        });
    }

    private void btnSalvarConfig_Click(object sender, EventArgs e)
    {
        SaveConfig();
    }

    private void btnGerarRps_Click(object sender, EventArgs e)
    {
        GerarRps();

        var path = Helpers.SelectFolder();
        if (path.IsEmpty()) return;

        openNFSe.NotasServico[0].Save(path);
    }

    private void btnGerarEnviarLoteRps_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            GerarRps();

            var numero = 1;
            if (InputBox.Show("Numero Lote", "Digite o numero do lote.", ref numero).Equals(DialogResult.Cancel)) return;

            var ret = openNFSe.Enviar(numero);
            ProcessarRetorno(ret);
        });
    }

    private void btnConsultarSituacao_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            var numero = 10;
            if (InputBox.Show("Numero Lote", "Digite o numero do lote.", ref numero).Equals(DialogResult.Cancel)) return;

            var protocolo = "0";
            if (InputBox.Show("Numero do Protocolo", "Digite o numero do protocolo.", ref protocolo).Equals(DialogResult.Cancel)) return;

            var ret = openNFSe.ConsultarSituacao(numero, protocolo);
            ProcessarRetorno(ret);
        });
    }

    private void btnConsultarLote_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            var numero = 10;
            if (InputBox.Show("Numero Lote", "Digite o numero do lote.", ref numero).Equals(DialogResult.Cancel)) return;

            var protocolo = "0";
            if (InputBox.Show("Numero do Protocolo", "Digite o numero do protocolo.", ref protocolo).Equals(DialogResult.Cancel)) return;

            var ret = openNFSe.ConsultarLoteRps(numero, protocolo);
            ProcessarRetorno(ret);
        });
    }

    private void btnConsultarNFSeRps_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            var numero = 10;
            if (InputBox.Show("Numero da RPS", "Digite o numero da RPS.", ref numero).Equals(DialogResult.Cancel)) return;

            var serie = "0";
            if (InputBox.Show("Serie da RPS", "Digite o numero da serie da RPS.", ref serie).Equals(DialogResult.Cancel)) return;

            var ret = openNFSe.ConsultaNFSeRps(numero, serie, TipoRps.RPS);
            ProcessarRetorno(ret);
        });
    }

    private void btnConsultarNFSePeriodo_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            var numero = 0;
            if (InputBox.Show("Numero da Nota", "Digite o numero da Nota", ref numero).Equals(DialogResult.Cancel)) return;

            var serie = "0";
            if (InputBox.Show("Serie da Nota", "Digite a serie da Nota", ref serie).Equals(DialogResult.Cancel)) return;

            RetornoConsultarNFSe ret;
            if (numero > 0)
                ret = openNFSe.ConsultaNFSe(numero, serie);
            else
                ret = openNFSe.ConsultaNFSePeriodo(DateTime.Today.AddDays(-7), DateTime.Today);

            ProcessarRetorno(ret);
        });
    }

    private void btnCancelarNFSe_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            var codigo = "0001";
            if (InputBox.Show("Código de cancelamento", "Código de cancelamento.", ref codigo).Equals(DialogResult.Cancel)) return;

            var numeroNFSe = "0";
            if (InputBox.Show("Numero NFSe", "Digite o numero da NFSe.", ref numeroNFSe).Equals(DialogResult.Cancel)) return;

            var serieNFSe = "0";
            if (InputBox.Show("Série NFSe", "Digite a série da NFSe.", ref serieNFSe).Equals(DialogResult.Cancel)) return;

            var motivo = "";
            if (InputBox.Show("Motivo Cancelamento", "Digite o motivo do cancelamento.", ref motivo).Equals(DialogResult.Cancel)) return;

            var codigoVerificacao = "";
            if (InputBox.Show("Código Verificação", "Digite o Código Verificação.", ref codigoVerificacao).Equals(DialogResult.Cancel)) return;

            var ret = openNFSe.CancelarNFSe(codigo, numeroNFSe, serieNFSe, 0, motivo, codigoVerificacao);
            ProcessarRetorno(ret);
        });
    }

    private void btnGerarEnviarLoteSinc_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            GerarRps();

            var numero = 1;
            if (InputBox.Show("Numero Lote", "Digite o numero do lote.", ref numero).Equals(DialogResult.Cancel)) return;

            var ret = openNFSe.Enviar(numero, true);
            ProcessarRetorno(ret);
        });
    }

    private void btnSelecionarSchema_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            txtSchemas.Text = Helpers.SelectFolder();
        });
    }

    private void btnPathXml_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            txtPathXml.Text = Helpers.SelectFolder();
        });
    }

    private void btnSelecionarArquivo_Click(object sender, EventArgs e)
    {
        LoadMunicipios();
    }

    private void btnAdicionar_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            var municipio = new OpenMunicipioNFSe();
            if (FormEdtMunicipio.Editar(municipio).Equals(DialogResult.Cancel)) return;

            AddMunicipio(municipio);
        });
    }

    private void btnCopiar_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            if (dgvCidades.SelectedRows.Count < 1) return;

            if (MessageBox.Show(@"Você tem certeza?", @"ACBrNFSe Demo", MessageBoxButtons.YesNo).Equals(DialogResult.No)) return;

            var municipio = ((OpenMunicipioNFSe)dgvCidades.SelectedRows[0].DataBoundItem).Clone();
            if (FormEdtMunicipio.Editar(municipio).Equals(DialogResult.Cancel)) return;

            AddMunicipio(municipio);
        });
    }

    private void btnDeletar_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            if (dgvCidades.SelectedRows.Count < 1) return;

            if (MessageBox.Show(@"Você tem certeza?", @"ACBrNFSe Demo", MessageBoxButtons.YesNo).Equals(DialogResult.No)) return;

            var cidade = (OpenMunicipioNFSe)dgvCidades.SelectedRows[0].DataBoundItem;
            ProviderManager.Municipios.Remove(cidade);

            dgvCidades.Rows.Remove(dgvCidades.SelectedRows[0]);
            UpdateCidades();
        });
    }

    private void btnCarregar_Click(object sender, EventArgs e)
    {
        LoadMunicipios();
    }

    private void btnSalvar_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            if (dgvCidades.Rows.Count < 1) return;

            var path = Helpers.SelectFolder();
            if (path.IsEmpty()) return;

            ProviderManager.Save(Path.Combine(path, "Municipios.nfse"));
        });
    }

    private void btnGetCertificate_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.MaxAllowed | OpenFlags.ReadOnly);

            var certificates = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, true)
                .Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, false);

            var certificadosSelecionados = X509Certificate2UI.SelectFromCollection(certificates, "Certificados Digitais",
                "Selecione o Certificado Digital para uso no aplicativo", X509SelectionFlag.SingleSelection);

            var certificado = certificadosSelecionados.Count < 1 ? null : certificadosSelecionados[0];
            txtNumeroSerie.Text = certificado?.GetSerialNumberString() ?? string.Empty;
        });
    }

    private void btnFindCertificate_Click(object sender, EventArgs e)
    {
        ExecuteSafe(() =>
        {
            var file = Helpers.OpenFile("Certificate Files (*.pfx)|*.pfx|All Files (*.*)|*.*", "Selecione o certificado");
            txtCertificado.Text = file;
        });
    }

    private void btnImprimirDANFSe_Click(object sender, EventArgs e)
    {
        GerarRps();
        openNFSe.Imprimir(o => o.MostrarPreview = true);
    }

    private void btnGerarPDF_Click(object sender, EventArgs e)
    {
        openNFSe.ImprimirPDF(o => o.NomeArquivo = "NFSe.pdf");
    }

    private void btnGerarHTML_Click(object sender, EventArgs e)
    {
        openNFSe.ImprimirPDF(o => o.NomeArquivo = "NFSe.html");
    }

    private void dgvCidades_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        ExecuteSafe(() =>
        {
            if (e.RowIndex < 0) return;

            var municipio = dgvCidades.Rows[e.RowIndex].DataBoundItem as OpenMunicipioNFSe;
            if (FormEdtMunicipio.Editar(municipio).Equals(DialogResult.Cancel)) return;

            LoadData();
        });
    }

    private void dgvCidades_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvCidades.RowCount <= 0 || e.RowIndex <= -1 || e.RowIndex > dgvCidades.Rows.Count - 1 || e.ColumnIndex <= -1)
            return;

        var municipio = (OpenMunicipioNFSe)dgvCidades.Rows[e.RowIndex].DataBoundItem;
        switch (dgvCidades.Columns[e.ColumnIndex].Name)
        {
            case "dgcCidade":
                e.Value = municipio.Nome;
                return;

            case "dgcUF":
                e.Value = municipio.UF.GetDFeValue();
                break;

            case "dgcCodigoIBGE":
                e.Value = municipio.Codigo.ToString();
                return;

            case "dgcProvedor":
                e.Value = municipio.Provedor.GetDescription();
                return;

            case "dgcLayout":
                e.Value = municipio.Provedor.GetDescription();
                return;

            case "dgcVersao":
                e.Value = municipio.Versao.GetDFeValue();
                return;
        }
    }

    #endregion EventHandlers

    #region ValueChanged

    private void txtCNPJ_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = txtCPFCNPJ.Text.OnlyNumbers();
    }

    private void txtIM_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.PrestadorPadrao.InscricaoMunicipal = txtIM.Text.OnlyNumbers();
    }

    private void txtRazaoSocial_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.PrestadorPadrao.RazaoSocial = txtRazaoSocial.Text;
    }

    private void txtFantasia_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.PrestadorPadrao.NomeFantasia = txtFantasia.Text;
    }

    private void txtFone_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.PrestadorPadrao.DadosContato.Telefone = txtFone.Text;
    }

    private void txtCEP_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Cep = txtCEP.Text;
    }

    private void txtEndereco_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Logradouro = txtEndereco.Text;
    }

    private void txtNumero_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Numero = txtNumero.Text;
    }

    private void txtComplemento_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Complemento = txtComplemento.Text;
    }

    private void txtBairro_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Bairro = txtBairro.Text;
    }

    private void cmbCidades_SelectedValueChanged(object sender, EventArgs e)
    {
        var municipio = cmbCidades.GetSelectedValue<OpenMunicipioNFSe>();
        if (municipio == null) return;

        txtCodCidade.Text = municipio.Codigo.ToString();
        txtProvedor.Text = municipio.Provedor.ToString();
        txtVersao.Text = municipio.Versao.GetDFeValue();

        openNFSe.Configuracoes.WebServices.CodigoMunicipio = municipio.Codigo;
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Municipio = municipio.Nome;
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio = municipio.Codigo;
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Uf = municipio.UF.ToString();
    }

    private void txtCertificado_TextChanged(object sender, EventArgs e)
    {
        if (txtCertificado.Text.IsEmpty()) return;

        txtNumeroSerie.Text = string.Empty;
        openNFSe.Configuracoes.Certificados.Certificado = txtCertificado.Text;
    }

    private void txtSenha_TextChanged(object sender, EventArgs e)
    {
        if (txtSenha.Text.IsEmpty()) return;

        openNFSe.Configuracoes.Certificados.Senha = txtSenha.Text;
    }

    private void txtNumeroSerie_TextChanged(object sender, EventArgs e)
    {
        if (txtNumeroSerie.Text.IsEmpty()) return;

        txtCertificado.Text = string.Empty;
        txtSenha.Text = string.Empty;
        openNFSe.Configuracoes.Certificados.Certificado = txtNumeroSerie.Text;
        openNFSe.Configuracoes.Certificados.Senha = string.Empty;
    }

    private void txtSchemas_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.Arquivos.PathSchemas = txtSchemas.Text;
    }

    private void chkSalvarArquivos_CheckedChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.Geral.Salvar = chkSalvarArquivos.Checked;
    }

    private void txtArquivoCidades_Click(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.Arquivos.ArquivoServicos = txtArquivoCidades.Text;
    }

    private void txtPathXml_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.Arquivos.PathSalvar = txtPathXml.Text;
    }

    private void txtArquivoCidades_TextChanged(object sender, EventArgs e)
    {
    }

    private void cmbAmbiente_SelectedValueChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.WebServices.Ambiente = cmbAmbiente.GetSelectedValue<DFeTipoAmbiente>();
    }

    private void txtWebserviceUsuario_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.WebServices.Usuario = txtWebserviceUsuario.Text;
    }

    private void txtWebserviceSenha_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.WebServices.Senha = txtWebserviceSenha.Text;
    }

    private void txtWebserviceChavePrivada_TextChanged(object sender, EventArgs e)
    {
        openNFSe.Configuracoes.WebServices.ChavePrivada = txtWebserviceChavePrivada.Text;
    }

    #endregion ValueChanged

    #region Overrides

    protected override void OnShown(EventArgs e)
    {
        openNFSe.Configuracoes.Geral.RetirarAcentos = true;
        openNFSe.Configuracoes.WebServices.Salvar = true;

        InitializeLog();
        this.Log().Debug("Log Iniciado");

        cmbAmbiente.EnumDataSource(DFeTipoAmbiente.Homologacao);
        LoadData();
        LoadConfig();

        base.OnShown(e);
    }

    #endregion Overrides

    private void GerarRps()
    {
        var municipio = cmbCidades.GetSelectedValue<OpenMunicipioNFSe>();
        if (municipio == null) return;

        var numeroRps = "1";
        if (InputBox.Show("Nº RPS", "Informe o número do RPS.", ref numeroRps).Equals(DialogResult.Cancel)) return;

        openNFSe.NotasServico.Clear();
        var nfSe = openNFSe.NotasServico.AddNew();

        nfSe.IdentificacaoRps.Numero = numeroRps;

        // Setar a serie de acordo com o provedor.
        switch (municipio.Provedor)
        {
            case NFSeProvider.ISSCuritiba:
                nfSe.IdentificacaoRps.Serie = "F";
                break;

            case NFSeProvider.ISSDSF:
                nfSe.IdentificacaoRps.Serie = "NF";
                nfSe.IdentificacaoRps.SeriePrestacao = "99";
                break;

            default:
                nfSe.IdentificacaoRps.Serie = "1";
                break;
        }

        nfSe.IdentificacaoRps.Tipo = TipoRps.RPS;
        nfSe.IdentificacaoRps.DataEmissao = DateTime.Now;
        nfSe.Competencia = DateTime.Now;
        nfSe.Situacao = SituacaoNFSeRps.Normal;
        nfSe.OptanteSimplesNacional = NFSeSimNao.Sim;
        nfSe.OptanteMEISimei = NFSeSimNao.Nao;

        // Setar a natureza de operação de acordo com o provedor.
        switch (municipio.Provedor)
        {
            case NFSeProvider.ISSDSF:
                nfSe.NaturezaOperacao = NaturezaOperacao.DSF.SemDeducao;
                break;

            default:
                nfSe.NaturezaOperacao = NaturezaOperacao.ABRASF.TributacaoNoMunicipio;
                break;
        }

        nfSe.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
        nfSe.IncentivadorCultural = NFSeSimNao.Nao;

        var itemListaServico = municipio.Provedor.IsIn(NFSeProvider.Betha, NFSeProvider.ISSe, NFSeProvider.ISSCuritiba) ? "0107" : "01.07";
        if (InputBox.Show("Item na lista de serviço", "Informe o item na lista de serviço.", ref itemListaServico).Equals(DialogResult.Cancel)) return;

        // Setar o cnae de acordo com o schema aceito pelo provedor.
        var cnae = GetCnae(municipio);
        if (InputBox.Show("CNAE", "Informe o codigo CNAE.", ref cnae).Equals(DialogResult.Cancel)) return;
        nfSe.Servico.CodigoCnae = cnae;

        var CodigoTributacaoMunicipio = municipio.Provedor.IsIn(NFSeProvider.SiapNet, NFSeProvider.ABase) ? "5211701" :
            municipio.Provedor.IsIn(NFSeProvider.Sigep) ? "1" : "01.07.00 / 00010700";

        nfSe.Servico.ItemListaServico = itemListaServico;
        nfSe.Servico.CodigoTributacaoMunicipio = CodigoTributacaoMunicipio;
        nfSe.Servico.Discriminacao = "MANUTENCAO TÉCNICA";
        nfSe.Servico.CodigoMunicipio = municipio.Codigo;
        nfSe.Servico.Municipio = municipio.Nome;
        nfSe.OutrasInformacoes = "VOCÊ PAGOU APROXIMADAMENTE R$ 41,15 DE TRIBUTOS FEDERAIS, R$ 8,26 DE TRIBUTOS MUNICIPAIS, R$ 256,57 PELOS PRODUTOS/SERVICOS, FONTE: IBPT.";
        if (municipio.Provedor.IsIn(NFSeProvider.SiapNet))
        {
            nfSe.Servico.ResponsavelRetencao = ResponsavelRetencao.Prestador;
            nfSe.Servico.MunicipioIncidencia = nfSe.Servico.CodigoMunicipio;
        }

        nfSe.Servico.Valores.ValorServicos = 100;
        nfSe.Servico.Valores.ValorDeducoes = 0;
        nfSe.Servico.Valores.ValorPis = 0;
        nfSe.Servico.Valores.ValorCofins = 0;
        nfSe.Servico.Valores.ValorInss = 0;
        nfSe.Servico.Valores.ValorIr = 0;
        nfSe.Servico.Valores.ValorCsll = 0;
        nfSe.Servico.Valores.IssRetido = SituacaoTributaria.Normal;
        nfSe.Servico.Valores.ValorIss = municipio.Provedor == NFSeProvider.SiapNet ? 2 : 0;
        nfSe.Servico.Valores.ValorOutrasRetencoes = 0;
        nfSe.Servico.Valores.BaseCalculo = 100;
        nfSe.Servico.Valores.Aliquota = 2;
        nfSe.Servico.Valores.ValorLiquidoNfse = 100;
        nfSe.Servico.Valores.ValorIssRetido = 0;
        nfSe.Servico.Valores.DescontoCondicionado = 0;
        nfSe.Servico.Valores.DescontoIncondicionado = 0;
        nfSe.ValorCredito = 0;

        if (municipio.Provedor == NFSeProvider.ISSDSF)
        {
            var servico = nfSe.Servico.ItemsServico.AddNew();
            servico.Descricao = "Teste";
            servico.Quantidade = 1M;
            servico.ValorTotal = 100;
            servico.Tributavel = NFSeSimNao.Sim;
        }

        nfSe.Tomador.CpfCnpj = "44854962283";
        nfSe.Tomador.InscricaoMunicipal = "";
        nfSe.Tomador.RazaoSocial = "Nome";

        nfSe.Tomador.Endereco.TipoLogradouro = "";
        nfSe.Tomador.Endereco.Logradouro = "INDEPENDENCIA";
        nfSe.Tomador.Endereco.Numero = "123";
        nfSe.Tomador.Endereco.Complemento = "SL 10";
        nfSe.Tomador.Endereco.Bairro = "VILA SEIXAS";
        nfSe.Tomador.Endereco.CodigoMunicipio = municipio.Codigo;
        nfSe.Tomador.Endereco.Municipio = municipio.Nome;
        nfSe.Tomador.Endereco.Uf = municipio.UF.ToString();
        nfSe.Tomador.Endereco.Cep = "14020010";
        nfSe.Tomador.Endereco.CodigoPais = 1058;
        nfSe.Tomador.Endereco.Pais = "BRASIL";

        nfSe.Tomador.DadosContato.DDD = "16";
        nfSe.Tomador.DadosContato.Telefone = "30111234";
        nfSe.Tomador.DadosContato.Email = "NOME@EMPRESA.COM.BR";
    }

    private static string GetCnae(OpenMunicipioNFSe municipio)
    {
        return municipio.Provedor switch
        {
            NFSeProvider.SiapNet => "5211701",
            NFSeProvider.Sintese => "5211701",
            NFSeProvider.ABase => "5211701",
            NFSeProvider.Pronim when municipio.Versao == VersaoNFSe.ve203 => "5211701",
            NFSeProvider.SigISS when municipio.Versao == VersaoNFSe.ve103 => "5211701",
            NFSeProvider.Ginfes => "5211701",
            NFSeProvider.Tiplan => "5211701",
            _ => "861010101"
        };
    }

    private void ProcessarRetorno(RetornoWebservice retorno)
    {
        rtLogResposta.Clear();

        switch (retorno)
        {
            case RetornoEnviar ret:
                rtLogResposta.AppendLine(ret.Sincrono ? "Metodo : Enviar Sincrono" : "Metodo : Enviar");
                rtLogResposta.AppendLine($"Data : {ret.Data:G}");
                rtLogResposta.AppendLine($"Lote : {ret.Lote}");
                rtLogResposta.AppendLine($"Protocolo : {ret.Protocolo}");
                rtLogResposta.AppendLine($"Sucesso : {ret.Sucesso}");
                break;

            case RetornoConsultarSituacao ret:
                rtLogResposta.AppendLine("Metodo : Consultar Situação Lote RPS");
                rtLogResposta.AppendLine($"Lote : {ret.Lote}");
                rtLogResposta.AppendLine($"Protocolo : {ret.Protocolo}");
                rtLogResposta.AppendLine($"Situação : {ret.Situacao}");
                rtLogResposta.AppendLine($"Sucesso : {ret.Sucesso}");
                break;

            case RetornoConsultarLoteRps ret:
                rtLogResposta.AppendLine("Metodo : Consultar Lote RPS");
                rtLogResposta.AppendLine($"Lote : {ret.Lote}");
                rtLogResposta.AppendLine($"Protocolo : {ret.Protocolo}");
                rtLogResposta.AppendLine($"Situação : {ret.Situacao}");
                rtLogResposta.AppendLine($"Sucesso : {ret.Sucesso}");

                if (!ret.Notas.IsNullOrEmpty())
                {
                    foreach (var nota in ret.Notas)
                    {
                        rtLogResposta.AppendLine($"NFSe : {nota.IdentificacaoNFSe.Numero}");
                        rtLogResposta.AppendLine($"Chave : {nota.IdentificacaoNFSe.Chave}");
                        rtLogResposta.AppendLine($"Data Emissão : {nota.IdentificacaoNFSe.DataEmissao:G}");
                    }
                }
                break;

            case RetornoConsultarNFSeRps ret:
                rtLogResposta.AppendLine("Metodo : Consultar NFSe por RPS");
                rtLogResposta.AppendLine($"Tipo : {ret.Tipo.GetDescription()}");
                rtLogResposta.AppendLine($"Serie : {ret.Serie}");
                rtLogResposta.AppendLine($"RPS : {ret.NumeroRps}");
                rtLogResposta.AppendLine($"Ano : {ret.AnoCompetencia}");
                rtLogResposta.AppendLine($"Mês : {ret.MesCompetencia}");
                rtLogResposta.AppendLine($"Sucesso : {ret.Sucesso}");

                if (ret.Nota != null)
                {
                    rtLogResposta.AppendLine($"NFSe : {ret.Nota.IdentificacaoNFSe.Numero}");
                    rtLogResposta.AppendLine($"Chave : {ret.Nota.IdentificacaoNFSe.Chave}");
                    rtLogResposta.AppendLine($"Data Emissão : {ret.Nota.IdentificacaoNFSe.DataEmissao:G}");
                }
                break;

            case RetornoConsultarNFSe ret:
                rtLogResposta.AppendLine("Metodo : Consultar NFSe");
                rtLogResposta.AppendLine($"Nome Intermediario : {ret.NomeIntermediario}");
                rtLogResposta.AppendLine($"CNPJ Intermediario : {ret.CPFCNPJIntermediario}");
                rtLogResposta.AppendLine($"IM Intermediario : {ret.IMIntermediario}");
                rtLogResposta.AppendLine($"CNPJ Tomador : {ret.CPFCNPJTomador}");
                rtLogResposta.AppendLine($"IM Tomador : {ret.IMTomador}");
                rtLogResposta.AppendLine($"Data Inicial : {(ret.Inicio ?? DateTime.MinValue):G}");
                rtLogResposta.AppendLine($"Data Final : {(ret.Fim ?? DateTime.MinValue):G}");
                rtLogResposta.AppendLine($"NFSe : {ret.NumeroNFse}");
                rtLogResposta.AppendLine($"Serie : {ret.SerieNFse}");
                rtLogResposta.AppendLine($"Pagina : {ret.Pagina}");
                rtLogResposta.AppendLine($"Proxima Pagina : {ret.ProximaPagina}");
                rtLogResposta.AppendLine($"Sucesso : {ret.Sucesso}");

                if (!ret.Notas.IsNullOrEmpty())
                {
                    foreach (var nota in ret.Notas)
                    {
                        rtLogResposta.AppendLine($"NFSe : {nota.IdentificacaoNFSe.Numero}");
                        rtLogResposta.AppendLine($"Chave : {nota.IdentificacaoNFSe.Chave}");
                        rtLogResposta.AppendLine($"Data Emissão : {nota.IdentificacaoNFSe.DataEmissao:G}");
                    }
                }
                break;

            case RetornoConsultarSequencialRps ret:
                rtLogResposta.AppendLine("Metodo : Consultar Sequencial RPS");
                rtLogResposta.AppendLine($"Serie : {ret.Serie}");
                rtLogResposta.AppendLine($"ùltimo RPS : {ret.UltimoNumero}");
                rtLogResposta.AppendLine($"Sucesso : {ret.Sucesso}");
                break;

            case RetornoCancelar ret:
                rtLogResposta.AppendLine("Metodo : Cancelar NFSe");
                rtLogResposta.AppendLine($"Codigo : {ret.CodigoCancelamento}");
                rtLogResposta.AppendLine($"Data : {ret.Data:G}");
                rtLogResposta.AppendLine($"Motivo : {ret.Motivo}");
                rtLogResposta.AppendLine($"NFSe : {ret.NumeroNFSe}");
                rtLogResposta.AppendLine($"Serie : {ret.SerieNFSe}");
                rtLogResposta.AppendLine($"Valor : {ret.ValorNFSe:C2}");
                rtLogResposta.AppendLine($"Sucesso : {ret.Sucesso}");
                break;

            case RetornoCancelarNFSeLote ret:
                rtLogResposta.AppendLine("Metodo : Cancelar NFSe Lote");
                rtLogResposta.AppendLine($"Lote : {ret.Lote}");
                rtLogResposta.AppendLine($"Sucesso : {ret.Sucesso}");
                break;

            case RetornoSubstituirNFSe ret:
                rtLogResposta.AppendLine("Metodo : Substituitr NFSe");
                rtLogResposta.AppendLine($"Codigo : {ret.CodigoCancelamento}");
                rtLogResposta.AppendLine($"Motivo : {ret.Motivo}");
                rtLogResposta.AppendLine($"NFSe : {ret.NumeroNFSe}");
                rtLogResposta.AppendLine($"Sucesso : {ret.Sucesso}");

                if (ret.Nota != null)
                {
                    rtLogResposta.AppendLine($"NFSe : {ret.Nota.IdentificacaoNFSe.Numero}");
                    rtLogResposta.AppendLine($"Chave : {ret.Nota.IdentificacaoNFSe.Chave}");
                    rtLogResposta.AppendLine($"Data Emissão : {ret.Nota.IdentificacaoNFSe.DataEmissao:G}");
                }
                break;
        }

        if (retorno.Alertas.Any())
        {
            rtLogResposta.JumpLine();
            rtLogResposta.AppendLine("Alerta(s):");
            foreach (var erro in retorno.Erros)
            {
                rtLogResposta.AppendLine($"Codigo : {erro.Codigo}");
                rtLogResposta.AppendLine($"Mensagem : {erro.Descricao}");
                rtLogResposta.AppendLine($"Correção : {erro.Correcao}");
                rtLogResposta.AppendLine("----------------------------------------------------");
            }
        }

        if (retorno.Erros.Any())
        {
            rtLogResposta.JumpLine();
            rtLogResposta.AppendLine("Erro(s):");
            foreach (var erro in retorno.Erros)
            {
                rtLogResposta.AppendLine($"Codigo : {erro.Codigo}");
                rtLogResposta.AppendLine($"Mensagem : {erro.Descricao}");
                rtLogResposta.AppendLine($"Correção : {erro.Correcao}");
                rtLogResposta.AppendLine("----------------------------------------------------");
            }
        }

        wbbDados.LoadXml(retorno.XmlEnvio);
        wbbEnvelopeEnvio.LoadXml(retorno.EnvelopeEnvio);
        wbbResposta.LoadXml(retorno.XmlRetorno);
        wbbRetorno.LoadXml(retorno.EnvelopeRetorno);
    }

    private void AddMunicipio(params OpenMunicipioNFSe[] municipios)
    {
        ProviderManager.Municipios.AddRange(municipios);
        LoadData();
    }

    private void LoadData()
    {
        dgvCidades.DataSource = null;
        dgvCidades.DataSource = ProviderManager.Municipios.ToArray();

        UpdateCidades();
    }

    private void LoadMunicipios()
    {
        ExecuteSafe(() =>
        {
            var arquivo = Helpers.OpenFile("Arquivo de cidades NFSe (*.nfse)|*.nfse|Todos os arquivos|*.*", "Selecione o arquivo de cidades");
            if (arquivo.IsEmpty()) return;

            ProviderManager.Load(arquivo);
            txtArquivoCidades.Text = arquivo;
            LoadData();
        });
    }

    private void UpdateCidades()
    {
        cmbCidades.MunicipiosDataSource();
    }

    private void InitializeLog()
    {
        var config = new LoggingConfiguration();
        var target = new RichTextBoxTarget
        {
            UseDefaultRowColoringRules = true,
            Layout = @"${date:format=dd/MM/yyyy HH\:mm\:ss} - ${message}",
            FormName = Name,
            ControlName = rtbLog.Name,
            AutoScroll = true
        };

        config.AddTarget("RichTextBox", target);
        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

        var infoTarget = new FileTarget
        {
            FileName = "${basedir:dir=Logs:file=ACBrNFSe.log}",
            Layout = "${processid}|${longdate}|${level:uppercase=true}|" +
                     "${event-context:item=Context}|${logger}|${message}",
            CreateDirs = true,
            Encoding = Encoding.UTF8,
            MaxArchiveFiles = 93,
            ArchiveEvery = FileArchivePeriod.Day,
            ArchiveNumbering = ArchiveNumberingMode.Date,
            ArchiveFileName = "${basedir}/Logs/Archive/${date:format=yyyy}/${date:format=MM}/NFSe_{{#}}.log",
            ArchiveDateFormat = "dd.MM.yyyy"
        };

        config.AddTarget("infoFile", infoTarget);
        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, infoTarget));
        LogManager.Configuration = config;
    }

    private void LoadConfig()
    {
        var cnpj = config.Get("PrestadorCPFCNPJ", string.Empty);
        if (!cnpj.IsEmpty())
        {
            txtCPFCNPJ.Text = cnpj.FormataCPFCNPJ();
        }

        txtIM.Text = config.Get("PrestadorIM", string.Empty);
        txtRazaoSocial.Text = config.Get("PrestadorRazaoSocial", string.Empty);
        txtFantasia.Text = config.Get("PrestadorFantasia", string.Empty);
        txtFone.Text = config.Get("PrestadorFone", string.Empty);
        txtCEP.Text = config.Get("PrestadorCEP", string.Empty);
        txtEndereco.Text = config.Get("PrestadorEndereco", string.Empty);
        txtNumero.Text = config.Get("PrestadorNumero", string.Empty);
        txtComplemento.Text = config.Get("PrestadorComplemento", string.Empty);
        txtBairro.Text = config.Get("PrestadorBairro", string.Empty);

        txtWebserviceUsuario.Text = config.Get("UsuarioWebservice", string.Empty);
        txtWebserviceSenha.Text = config.Get("SenhaWebservice", string.Empty);
        txtWebserviceChavePrivada.Text = config.Get("ChavePrivadaWebservice", string.Empty);

        var codMunicipio = config.Get("Municipio", 0);
        if (codMunicipio > 0)
        {
            var municipio = ProviderManager.Municipios.SingleOrDefault(x => x.Codigo == codMunicipio);
            if (municipio != null)
            {
                cmbCidades.SetSelectedValue(municipio);
            }
        }

        cmbAmbiente.SetSelectedValue(config.Get("Ambiente", DFeTipoAmbiente.Homologacao));

        txtCertificado.Text = config.Get("Certificado", string.Empty);
        txtSenha.Text = config.Get("Senha", string.Empty);
        txtNumeroSerie.Text = config.Get("NumeroSerie", string.Empty);

        txtSchemas.Text = config.Get("PastaSchemas", string.Empty);
        txtArquivoCidades.Text = config.Get("ArquivoCidades", string.Empty);
        chkSalvarArquivos.Checked = config.Get("SalvarNfse", string.Empty) == "1";
        txtPathXml.Text = config.Get("CaminhoXML", string.Empty);
    }

    private void SaveConfig()
    {
        config.Set("PrestadorCPFCNPJ", txtCPFCNPJ.Text.OnlyNumbers());
        config.Set("PrestadorIM", txtIM.Text.OnlyNumbers());
        config.Set("PrestadorRazaoSocial", txtRazaoSocial.Text);
        config.Set("PrestadorFantasia", txtFantasia.Text);
        config.Set("PrestadorFone", txtFone.Text);
        config.Set("PrestadorCEP", txtCEP.Text);
        config.Set("PrestadorEndereco", txtEndereco.Text);
        config.Set("PrestadorNumero", txtNumero.Text);
        config.Set("PrestadorComplemento", txtComplemento.Text);
        config.Set("PrestadorBairro", txtBairro.Text);

        config.Set("Municipio", txtCodCidade.Text.OnlyNumbers());

        config.Set("Ambiente", cmbAmbiente.GetSelectedValue<DFeTipoAmbiente>());

        config.Set("Certificado", txtCertificado.Text);
        config.Set("Senha", txtSenha.Text);
        config.Set("NumeroSerie", txtNumeroSerie.Text);

        config.Set("UsuarioWebservice", txtWebserviceUsuario.Text);
        config.Set("SenhaWebservice", txtWebserviceSenha.Text);
        config.Set("ChavePrivadaWebservice", txtWebserviceChavePrivada.Text);

        config.Set("PastaSchemas", txtSchemas.Text);
        config.Set("ArquivoCidades", txtArquivoCidades.Text);

        config.Set("SalvarNfse", chkSalvarArquivos.Checked ? "1" : "0");
        config.Set("CaminhoXML", txtPathXml.Text);

        config.Save();
    }

    private void ExecuteSafe(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            lblStatus.Text = exception.Message;
            rtLogResposta.Clear();
            rtLogResposta.AppendLine($"Erro : {exception.Message}");
        }
    }

    #endregion Methods
}
