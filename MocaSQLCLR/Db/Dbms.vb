
Imports System.Configuration
Imports System.Data.Common
Imports System.Threading
Imports Moca.Util

Namespace Db

	''' <summary>
	''' DBMS
	''' </summary>
	''' <remarks>
	''' DataBase Management System�̗��B
	''' �f�[�^�x�[�X���\�z�E�^�p���邽�߂ɗp������Ǘ��\�t�g�E�F�A�̂��ƂŁA
	''' ���̃N���X�łP�ڑ����\���܂��B
	''' </remarks>
	Public Class Dbms

		''' <summary>�\���t�@�C���̐ڑ�������Z�N�V��������DB�ڑ���������Ǘ�</summary>
		Private _dbSetting As DbSetting
		''' <summary>�\���t�@�C���̐ڑ�������Z�N�V�������̖��O�t���ŒP��̐ڑ��������\���܂��B</summary>
		Private _connectionStringSettings As ConnectionStringSettings
		''' <summary>�v���o�C�_�̃f�[�^ �\�[�X �N���X�̎����̃C���X�^���X���쐬���邽�߂̃��\�b�h�̃Z�b�g��\���܂��B</summary>
		Private _providerFactory As DbProviderFactory

		Private _helperFactory As Helper.DbAccessHelperFactory

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="connectionString">ConnectionString</param>
		''' <remarks>
		''' app.config ����ڑ�������A�v���p�C�_���擾���R�l�N�V�������쐬����
		''' </remarks>
		''' <exception cref="ArgumentException">
		''' DB�ڑ�����ׂ̐ڑ������񂪐ݒ肳��Ă��Ȃ��Ƃ��ɔ�������B
		''' </exception>
		Public Sub New(ByVal connectionString As String)
			Try
				' �ڑ��������app.config�t�@�C������擾
				_connectionStringSettings = New ConnectionStringSettings(connectionString, connectionString, "System.Data.SqlClient")
				_dbSetting = New DbSetting(_connectionStringSettings)

				' DB�A�N�Z�X�p�̃v���p�C�_���C���X�^���X��
				_providerFactory = DbProviderFactories.GetFactory(_connectionStringSettings.ProviderName)

				_helperFactory = New Helper.DbAccessHelperFactory(_dbSetting)

			Catch ex As ArgumentException
				Throw ex
			Finally
			End Try
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="name">�ڑ��於��</param>
		''' <param name="providerName">�v���p�C�_��</param>
		''' <param name="connectionString">�ڑ�������</param>
		''' <remarks>
		''' �w�肳�ꂽ�������ɐڑ�������A�v���p�C�_���擾���R�l�N�V�������쐬����
		''' </remarks>
		Public Sub New(ByVal name As String, ByVal providerName As String, ByVal connectionString As String)
			Try
				' DB�A�N�Z�X�p�̃v���p�C�_���C���X�^���X��
				_providerFactory = DbProviderFactories.GetFactory(providerName)

				_connectionStringSettings = New ConnectionStringSettings(name, connectionString, providerName)
				_dbSetting = New DbSetting(_connectionStringSettings)

				_helperFactory = New Helper.DbAccessHelperFactory(_dbSetting)

			Catch ex As Exception
				Throw ex
			Finally
			End Try
		End Sub

#End Region

#Region " �v���p�e�B "

		''' <summary>�\���t�@�C���̐ڑ�������Z�N�V��������DB�ڑ���������Ǘ�</summary>
		Public ReadOnly Property Setting() As DbSetting
			Get
				Return _dbSetting
			End Get
		End Property

		''' <summary>�\���t�@�C���̐ڑ�������Z�N�V�������̖��O�t���ŒP��̐ڑ��������\���܂��B</summary>
		Public ReadOnly Property ConnectionStringSettings() As ConnectionStringSettings
			Get
				Return _connectionStringSettings
			End Get
		End Property

		''' <summary>�v���o�C�_�̃f�[�^ �\�[�X �N���X�̎����̃C���X�^���X���쐬���邽�߂̃��\�b�h�̃Z�b�g��\���܂��B</summary>
		Public ReadOnly Property ProviderFactory() As DbProviderFactory
			Get
				Return _providerFactory
			End Get
		End Property

#End Region

		''' <summary>
		''' �w���p�[�N���X�̃C���X�^���X��
		''' </summary>
		''' <param name="dba">DB�փA�N�Z�X����C���X�^���X</param>
		''' <remarks></remarks>
		Public Function GetHelper(ByVal dba As IDao) As IDbAccessHelper
			Return _helperFactory.Create(dba)
		End Function

		''' <summary>
		''' �V�����ڑ����쐬����
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function CreateConnection() As IDbConnection
			Dim conn As IDbConnection

			conn = Me.ProviderFactory.CreateConnection()
			conn.ConnectionString = Me.ConnectionStringSettings.ConnectionString

			Return conn
		End Function

		''' <summary>
		''' �V���� DBAccess �C���X�^���X���쐬����B
		''' </summary>
		''' <returns>�V���� DBAccess �C���X�^���X</returns>
		''' <remarks></remarks>
		Public Function CreateDbAccess() As IDbAccess
			Dim dba As IDbAccess

			dba = New DbAccess(Me)

			Return dba
		End Function

		''' <summary>
		''' �V���� DBAccess �C���X�^���X���쐬����B
		''' </summary>
		''' <returns>�V���� DBAccess �C���X�^���X</returns>
		''' <remarks></remarks>
		Public Function CreateDbAccess(Of T)() As T
			Dim dba As Object

			' �^�`�F�b�N
			Dim ok As Boolean
			For Each item As Type In GetType(T).GetInterfaces
				If item.Equals(GetType(IDbAccess)) Then
					ok = True
				End If
			Next
			If Not ok Then
				Throw New ArgumentException(GetType(T).FullName & " �́A" & GetType(IDbAccess).FullName & " �����������N���X�ł͂���܂���B")
			End If

			dba = ClassUtil.NewInstance(GetType(T), New Object() {Me})

			Return DirectCast(dba, T)
		End Function

	End Class

End Namespace
