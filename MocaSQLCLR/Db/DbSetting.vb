
Imports System.Configuration
Imports System.ComponentModel
Imports System.Data.Common

Imports Moca.Security

Namespace Db

	''' <summary>
	''' �\���t�@�C���̐ڑ�������Z�N�V��������DB�ڑ���������Ǘ����܂��B
	''' </summary>
	''' <remarks>
	''' ����̃R���s���[�^�A�A�v���P�[�V�����A�܂��̓��\�[�X�ɓK�p�ł���\���t�@�C����DB�ڑ��������ۑ�������A�Ǎ��񂾂肵�܂��B
	''' </remarks>
	Public Class DbSetting

		''' <summary>����̃R���s���[�^�A�A�v���P�[�V�����A�܂��̓��\�[�X�ɓK�p�ł���\���t�@�C��</summary>
		Private _config As System.Configuration.Configuration

		''' <summary>�ڑ�������̖���</summary>
		Private _name As String
		''' <summary>�v���p�C�_�N���X��</summary>
		Private _providerName As String
		''' <summary>�T�[�o�[��</summary>
		Private _server As String
		''' <summary>�f�[�^�x�[�X��</summary>
		Private _database As String
		''' <summary>�ڑ����[�U�[��</summary>
		Private _user As String
		''' <summary>�ڑ����[�U�[�̃p�X���[�h</summary>
		Private _password As String

		''' <summary>OleDb�v���p�C�_�N���X��</summary>
		Private _oleDbProviderName As String

		''' <summary>�ڑ�������̖��́i�J�����g�ۑ��p�j</summary>
		Private _currentName As String

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �f�t�H���g�R���X�g���N�^
		''' </summary>
		''' <remarks></remarks>
		Public Sub New()
			_name = String.Empty
			_providerName = String.Empty
			_server = String.Empty
			_database = String.Empty
			_user = String.Empty
			_password = String.Empty
			_currentName = String.Empty
			_oleDbProviderName = String.Empty
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="setting">�\���t�@�C���̐ڑ�������Z�N�V�������̖��O�t���ŒP��̐ڑ��������\���N���X</param>
		''' <remarks></remarks>
		Public Sub New(ByVal setting As ConnectionStringSettings)
			Me.New()
			moveValues(setting)
		End Sub

#End Region

#Region " �v���p�e�B "

		''' <summary>
		''' �\���t�@�C���v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		<Browsable(False)> _
		Public Property Config() As System.Configuration.Configuration
			Get
				Return _config
			End Get
			Set(ByVal value As System.Configuration.Configuration)
				_config = value
			End Set
		End Property

		''' <summary>
		''' �ڑ�������̖��̃v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		<Browsable(False)> _
		Public Property Name() As String
			Get
				Return _name
			End Get
			Set(ByVal value As String)
				_name = value
			End Set
		End Property

		''' <summary>
		''' �v���p�C�_�N���X���v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		<Browsable(False) _
		, Category("Database") _
		, Description("Provider to connect it with data base.")> _
		Public Property ProviderName() As String
			Get
				Return _providerName
			End Get
			Set(ByVal value As String)
				_providerName = value
			End Set
		End Property

		''' <summary>
		''' �T�[�o�[���v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		<Category("Database") _
		, Description("Server name at connection destination.")> _
		Public Property Server() As String
			Get
				Return _server
			End Get
			Set(ByVal value As String)
				_server = value
			End Set
		End Property

		''' <summary>
		''' �f�[�^�x�[�X���v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		<Category("Database") _
		, Description("Database name at connection destination.")> _
		Public Property Database() As String
			Get
				Return _database
			End Get
			Set(ByVal value As String)
				_database = value
			End Set
		End Property

		''' <summary>
		''' �ڑ����[�U�[���v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		<Category("Database") _
		, Description("Connected user name.")> _
		Public Property User() As String
			Get
				Return _user
			End Get
			Set(ByVal value As String)
				_user = value
			End Set
		End Property

		''' <summary>
		''' �ڑ����[�U�[�̃p�X���[�h�v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		<Category("Database") _
		, Description("Password of user who connects it.") _
		, PasswordPropertyText(True)> _
		Public Property Password() As String
			Get
				Return _password
			End Get
			Set(ByVal value As String)
				_password = value
			End Set
		End Property

		''' <summary>
		''' OleDb�ڑ����̃v���o�C�_�[
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		<Category("Database") _
		, Description("OleDb Provider Name.")> _
		Public Property OleDbProviderName() As String
			Get
				Return _oleDbProviderName
			End Get
			Set(ByVal value As String)
				_oleDbProviderName = value
			End Set
		End Property

#End Region

		''' <summary>
		''' �\���t�@�C���̐ڑ�������Z�N�V������Ԃ��܂��B
		''' </summary>
		''' <param name="name">ConnectionStringSettings</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetSection(ByVal name As String) As ConnectionStringSettings
			Dim csSection As ConnectionStringsSection = Config.ConnectionStrings
			Return csSection.ConnectionStrings.Item(name)
		End Function

		''' <summary>
		''' �\���t�@�C���֐ڑ�������Z�N�V������ǉ����܂��B
		''' </summary>
		''' <remarks></remarks>
		Public Sub AddSection()
			GetSection().ConnectionStrings.Add(cnvSetting())
		End Sub

		''' <summary>
		''' �\���t�@�C���̐ڑ�������Z�N�V������ύX���܂��B
		''' </summary>
		''' <remarks></remarks>
		Public Sub ModSection()
			DelSection(_currentName)
			GetSection().ConnectionStrings.Add(cnvSetting())
		End Sub

		''' <summary>
		''' �\���t�@�C���̐ڑ�������Z�N�V�������폜���܂��B
		''' </summary>
		''' <param name="name">�Z�N�V��������</param>
		''' <remarks></remarks>
		Public Sub DelSection(ByVal name As String)
			GetSection().ConnectionStrings.Remove(name)
		End Sub

		''' <summary>
		''' �ڑ�������Z�N�V������
		''' </summary>
		''' <param name="name">�Z�N�V��������</param>
		''' <remarks></remarks>
		Public Sub Read(ByVal name As String)
			moveValues(GetSection(name))
		End Sub

		''' <summary>
		''' �\���t�@�C����ۑ����܂��B
		''' </summary>
		''' <remarks></remarks>
		Public Sub Save()
			Config.Save(ConfigurationSaveMode.Modified)
		End Sub

		' ''' <summary>
		' ''' �\���t�@�C�����Í������ĕۑ����܂��B
		' ''' </summary>
		' ''' <remarks></remarks>
		'Public Sub SaveDPAPI()
		'	Dim dpapi As DPAPIConfiguration

		'	If GetSection.SectionInformation.IsProtected Then
		'		Save()
		'		Exit Sub
		'	End If

		'	' �Í���
		'	dpapi = New DPAPIConfiguration(Config)
		'	dpapi.ProtectConnectionStrings()
		'End Sub

		''' <summary>
		''' �ݒ���e��ޔ����܂��B
		''' </summary>
		''' <param name="setting"></param>
		''' <remarks></remarks>
		Protected Sub moveValues(ByVal setting As ConnectionStringSettings)
			Dim builder As DbConnectionStringBuilder
			Dim buf As Object = Nothing

			_name = setting.Name
			_currentName = _name

			_providerName = setting.ProviderName

			builder = New DbConnectionStringBuilder
			builder.ConnectionString = setting.ConnectionString
			If builder.TryGetValue("Data Source", buf) Then
				_server = CStr(buf)
			End If
			If builder.TryGetValue("Initial Catalog", buf) Then
				_database = CStr(buf)
			End If
			If builder.TryGetValue("User ID", buf) Then
				_user = CStr(buf)
			End If
			If builder.TryGetValue("Password", buf) Then
				_password = CStr(buf)
			End If
			If builder.TryGetValue("Provider", buf) Then
				_oleDbProviderName = CStr(buf)
			End If
		End Sub

		''' <summary>
		''' �ڑ�������Z�N�V������Ԃ��܂��B
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function getSection() As ConnectionStringsSection
			Return Config.ConnectionStrings
		End Function

		''' <summary>
		''' �����ŕێ����Ă���f�[�^��ConnectionStringSettings�֕ϊ�����B
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function cnvSetting() As ConnectionStringSettings
			Dim builder As DbConnectionStringBuilder

			builder = New DbConnectionStringBuilder()
			builder.Add("Data Source", _server)
			builder.Add("Initial Catalog", _database)
			builder.Add("User ID", _user)
			builder.Add("Password", _password)
			builder.Add("Persist Security Info", True)
			If Trim(_oleDbProviderName).Length <= 0 Then
				builder.Add("Provider", _oleDbProviderName)
			End If
			If Trim(_name).Length <= 0 Then
				_name = _server & "." & _database
			End If

			Dim setting As ConnectionStringSettings

			setting = New ConnectionStringSettings()
			setting.Name = _name
			setting.ProviderName = _providerName
			setting.ConnectionString = builder.ConnectionString

			_currentName = setting.Name
			Return setting
		End Function

	End Class

End Namespace
