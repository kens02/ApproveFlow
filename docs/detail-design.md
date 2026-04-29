# Detail Design

## クラス設計
- `LeaveRequest`: 申請集約
- `WorkflowStep`: 承認ステップ状態
- `LeaveBalance`: 残数
- `ApprovalRoute`: 承認ルート定義
- `LeaveRequestService`: 申請/承認ユースケース

## モジュール設計
- Application/Services: ユースケース実装
- Infrastructure/Persistence: 永続化実装
- Api/Controllers: 入出力

## インターフェース仕様
- `ILeaveRequestRepository`
- `IApprovalRouteRepository`
- `ILeaveBalanceRepository`
- `INotificationService`
- `IUnitOfWork`

## 処理フロー
1. 申請作成時に入力検証
2. 申請者ルート読み込み
3. WorkflowStep 生成
4. 提出時は第1承認者へ通知
5. 承認ごとに次ステップ遷移
6. 最終承認で残数消費

## 状態遷移
Draft -> Submitted -> InReview -> Approved
InReview -> Rejected
Draft/InReview -> Cancelled

## エラー設計
- バリデーション違反: `ArgumentException`
- 承認権限違反: `UnauthorizedAccessException`
- データ未存在: `KeyNotFoundException`
- 業務ルール違反: `InvalidOperationException`

## ログ仕様
- 通知送信情報を Info ログ
- 将来はアクション履歴と相関ID連携

## バリデーション仕様
- 開始 < 終了
- 時間単位は15分刻み
- 日単位は465分単位
- 承認ルートは最小4段
- 承認順は1から連番
