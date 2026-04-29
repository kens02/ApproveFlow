# ApproveFlow (PoC)

休暇申請の承認決裁フローを扱う PoC システムです。バックエンド API と Next.js フロントエンドを同梱しています。

## 技術スタック
- Backend: ASP.NET Core Web API (.NET 7)
- Frontend: Next.js 16 + TypeScript
- Data: InMemory Repository（将来 MySQL へ差し替え可能）
- Auth: JWT Bearer
- Test: xUnit + Vitest

## セットアップ
```bash
./setup.sh
```

## 実行
1. API
```bash
dotnet run --project src/ApproveFlow.Api/ApproveFlow.Api.csproj
```
2. Frontend
```bash
cd frontend
npm run dev
```

## テスト
```bash
dotnet test ApproveFlow.sln
cd frontend && npm run test
```

## API 概要
- `POST /api/leave-requests`
- `POST /api/leave-requests/{id}/approve`
- `POST /api/leave-requests/{id}/reject`
- `POST /api/leave-requests/{id}/cancel`
- `PUT /api/routes`
- `GET /api/balances/{userId}`
- `GET /api/dashboard/{userId}?year=2026&fiscalYear=true`

詳細は `docs/api-spec.md` を参照してください。
