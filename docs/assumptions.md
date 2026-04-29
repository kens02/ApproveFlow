# Assumptions

- `docs/constraints.md` は存在しなかったため、制約条件は `docs/requirements.md` と一般的な PoC ベストプラクティスから補完した。
- 認証方式は JWT Bearer を採用し、ロールは `Applicant` / `Approver` / `Administrator` とした。
- PoC のため承認ルート上限は 20 段までに設定した（要件の「6名以上拡張可能」を満たす実装上の安全上限）。
- 通知は SMTP/Graph の代替として抽象化し、実装はログ出力で代替した。
- DB は InMemory 実装で提供し、リポジトリインターフェースにより MySQL 差し替え可能な構造にした。
- PDF 出力は本PoCでは API レイヤー未実装とし、設計書に HTML->PDF 変換拡張ポイントを記載した。
- 1日 = 465分、時間単位 15分刻み、日単位は 465 分単位でバリデーションする。
