# API Spec

## 認証方式
- JWT Bearer (`Authorization: Bearer <token>`)

## エンドポイント一覧
- `POST /api/leave-requests`
- `POST /api/leave-requests/{id}/approve`
- `POST /api/leave-requests/{id}/reject`
- `POST /api/leave-requests/{id}/cancel`
- `GET /api/leave-requests/{id}`
- `GET /api/leave-requests/applicant/{applicantId}`
- `GET /api/leave-requests/approver/{approverId}`
- `PUT /api/routes`
- `GET /api/balances/{userId}`
- `GET /api/dashboard/{userId}?year=2026&fiscalYear=true`

## リクエスト例
`POST /api/leave-requests`
```json
{
  "applicantId": "u1",
  "applicantName": "User One",
  "leaveType": "Annual",
  "unitType": 1,
  "startDateTime": "2026-04-01T00:00:00Z",
  "endDateTime": "2026-04-02T07:45:00Z",
  "reason": "family event",
  "attachmentFileName": null,
  "submitImmediately": true
}
```

## レスポンス例
```json
{
  "id": "b8d4aabd-25f1-4d61-a329-2d2e1f6b3c7a",
  "applicantId": "u1",
  "status": 2,
  "currentStepOrder": 1,
  "requestedDays": 1.0
}
```

## ステータスコード
- `200 OK`
- `204 NoContent`
- `400 BadRequest`
- `401 Unauthorized`
- `403 Forbidden`
- `404 NotFound`
- `409 Conflict`
