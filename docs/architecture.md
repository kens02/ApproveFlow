# Architecture

## 概要
ApproveFlow PoC は Clean Architecture を採用し、`Domain -> Application -> Infrastructure -> Api` の依存方向を維持する。

## レイヤ構成
- Domain: エンティティ、列挙、業務定数、純粋ルール
- Application: ユースケースサービス、DTO、抽象インターフェース
- Infrastructure: リポジトリ実装、通知実装、DI構成
- Api: 認証、認可、HTTPエンドポイント

## 依存ルール
- Domain は他レイヤ参照禁止
- Application は Domain のみ参照
- Infrastructure は Application/Domain を参照して実装注入
- Api は Application へ直接依存せず DI 経由で利用

## 全体構成図（テキスト）
Client -> API Controllers -> Application Services -> Repository Interfaces -> Infrastructure Repositories -> InMemory/MySQL

## セキュリティ
- JWT Bearer 認証
- ロールベース認可（Administrator 限定 API あり）
- 監査用途として ActionHistory 保存

## 拡張点
- `ILeaveRequestRepository` を MySQL 実装へ差し替え
- `INotificationService` を SMTP/Graph 実装へ差し替え
- 印刷 API を追加して HTML->PDF を実装
