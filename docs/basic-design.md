# Basic Design

## システム概要
休暇申請・承認・決裁・残数管理・集計を提供する承認決裁システム PoC。

## 目的
紙運用を排除し、申請〜決裁の可視化とトレーサビリティを確保する。

## スコープ
- 休暇申請登録
- 承認ルート設定
- 段階承認/差戻し/取消
- 残数計算
- ダッシュボード集計

## 用語定義
- 申請: 休暇取得の意思登録
- 承認: 中間承認
- 決裁: 最終承認
- 差戻し: 申請却下

## 機能一覧
- 申請 API
- 承認 API
- 差戻し API
- 取消 API
- ルート設定 API
- 残数照会 API
- ダッシュボード API

## 非機能要件整理
- 応答目標: 通常3秒以内
- 認証必須
- RBAC
- 操作履歴記録

## 全体構成図
Client -> ASP.NET API -> Application -> Repository -> InMemory/MySQL

## 技術選定理由
- ASP.NET Core: 型安全と保守性
- Clean Architecture: 疎結合
- xUnit: 標準的なテスト基盤

## コンポーネント構成
- Controllers
- Services
- Repositories
- Notification Adapter

## 外部依存
- JWT
- 将来: MySQL, SMTP/Graph, PDF 変換ライブラリ

## 制約事項
- 本PoC は InMemory 実装
- 印刷 API は拡張ポイント定義まで
