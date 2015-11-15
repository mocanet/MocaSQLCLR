
Imports Moca.Di

Namespace Db.SQLCLR

	''' <summary>
	''' SQLストアドプロシージャを実行する抽象クラス
	''' </summary>
	''' <remarks></remarks>
	Public MustInherit Class MocaStoredProcedures

		''' <summary>
		''' 実行
		''' </summary>
		''' <param name="target"></param>
		''' <remarks></remarks>
		Shared Sub Execute(ByVal target As ISQLCLRProcedure, Optional ByVal transactionType As Config.TransactionType = Config.TransactionType.Scope)
			Try
				' Moca初期化
				MocaContainerFactory.Init()

				' トランザクションタイプの指定
				Config.MocaConfiguration.Section.TransactionType = transactionType

				' インジェクト
				Dim injector As MocaInjector = New MocaInjector()
				injector.Inject(target)

				' 実行
				target.Execute()
			Finally
				' Moca後始末
				MocaContainerFactory.Destroy()
			End Try
		End Sub

	End Class

End Namespace
