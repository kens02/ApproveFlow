# Data Model

## ER図（テキスト）
- User(1) - (N) LeaveRequest
- LeaveRequest(1) - (N) WorkflowStep
- LeaveRequest(1) - (N) ActionHistory
- User(1) - (1) LeaveBalance
- User(1) - (N) ApprovalRouteStep

## テーブル定義
- `leave_requests(id, applicant_id, leave_type, unit_type, start_at, end_at, reason, status, attachment)`
- `workflow_steps(id, request_id, step_order, approver_id, status, actioned_at, comment)`
- `action_histories(id, request_id, action_type, user_id, comment, timestamp)`
- `leave_balances(user_id, granted_days, used_days, carry_over_days)`
- `approval_route_steps(id, applicant_id, step_order, approver_id, approver_name, notification_email, mail_template)`

## インデックス戦略
- `leave_requests(applicant_id, start_at desc)`
- `workflow_steps(request_id, step_order)`
- `workflow_steps(approver_id, status)`
- `action_histories(request_id, timestamp desc)`

## 正規化方針
- 3NF を基本とし、マスタ属性とトランザクション属性を分離。

## トランザクション方針
- 申請作成: Request + WorkflowStep + ActionHistory を同一トランザクション
- 承認処理: Step 更新 + Request 状態更新 + ActionHistory + Balance 更新を同一トランザクション
