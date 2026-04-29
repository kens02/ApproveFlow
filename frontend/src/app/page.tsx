"use client";

import { FormEvent, useMemo, useState } from "react";
import { api } from "@/lib/api";
import { DashboardSummary, LeaveBalanceSummary, LeaveRequestSummary, RouteStepInput } from "@/lib/types";

const statusMap: Record<number, string> = {
  0: "Draft",
  1: "Submitted",
  2: "InReview",
  3: "Approved",
  4: "Rejected",
  5: "Cancelled",
};

function localIso(value: string) {
  return new Date(value).toISOString();
}

export default function Home() {
  const [token, setToken] = useState("");
  const [applicantId, setApplicantId] = useState("u1");
  const [applicantName, setApplicantName] = useState("User One");
  const [approverId, setApproverId] = useState("a1");
  const [leaveType, setLeaveType] = useState("Annual");
  const [unitType, setUnitType] = useState<0 | 1>(1);
  const [startAt, setStartAt] = useState("2026-04-01T09:00");
  const [endAt, setEndAt] = useState("2026-04-01T16:45");
  const [reason, setReason] = useState("Family event");
  const [submitImmediately, setSubmitImmediately] = useState(true);
  const [requestIdAction, setRequestIdAction] = useState("");
  const [actionComment, setActionComment] = useState("");
  const [routeInput, setRouteInput] = useState<RouteStepInput[]>([
    { stepOrder: 1, approverId: "a1", approverName: "Approver 1", notificationEmail: "a1@example.com", mailTemplate: "" },
    { stepOrder: 2, approverId: "a2", approverName: "Approver 2", notificationEmail: "a2@example.com", mailTemplate: "" },
    { stepOrder: 3, approverId: "a3", approverName: "Approver 3", notificationEmail: "a3@example.com", mailTemplate: "" },
    { stepOrder: 4, approverId: "a4", approverName: "Approver 4", notificationEmail: "a4@example.com", mailTemplate: "" },
  ]);

  const [requests, setRequests] = useState<LeaveRequestSummary[]>([]);
  const [approverRequests, setApproverRequests] = useState<LeaveRequestSummary[]>([]);
  const [balance, setBalance] = useState<LeaveBalanceSummary | null>(null);
  const [dashboard, setDashboard] = useState<DashboardSummary | null>(null);
  const [message, setMessage] = useState("Ready");

  const barMax = useMemo(() => {
    if (!dashboard || dashboard.monthlyStats.length === 0) {
      return 1;
    }
    return Math.max(...dashboard.monthlyStats.map((x) => x.usedDays), 1);
  }, [dashboard]);

  async function wrap(label: string, fn: () => Promise<void>) {
    try {
      setMessage(`${label}...`);
      await fn();
      setMessage(`${label}: success`);
    } catch (error) {
      const text = error instanceof Error ? error.message : "unknown error";
      setMessage(`${label}: ${text}`);
    }
  }

  function onSaveToken(e: FormEvent) {
    e.preventDefault();
    window.localStorage.setItem("approveflow-token", token);
    setMessage("Token saved in localStorage");
  }

  function onLoadToken() {
    const v = window.localStorage.getItem("approveflow-token") ?? "";
    setToken(v);
    setMessage(v ? "Token loaded" : "No token in localStorage");
  }

  async function onCreateRequest(e: FormEvent) {
    e.preventDefault();
    await wrap("Create request", async () => {
      await api.createRequest(token, {
        applicantId,
        applicantName,
        leaveType,
        unitType,
        startDateTime: localIso(startAt),
        endDateTime: localIso(endAt),
        reason,
        attachmentFileName: null,
        submitImmediately,
      });
      setRequests(await api.getApplicantRequests(token, applicantId));
    });
  }

  async function onSetRoute() {
    await wrap("Set route", async () => {
      await api.setRoute(token, {
        applicantId,
        steps: routeInput,
      });
    });
  }

  async function onLoadApplicant() {
    await wrap("Load applicant requests", async () => {
      setRequests(await api.getApplicantRequests(token, applicantId));
    });
  }

  async function onLoadApprover() {
    await wrap("Load approver requests", async () => {
      setApproverRequests(await api.getApproverRequests(token, approverId));
    });
  }

  async function onApprove() {
    await wrap("Approve", async () => {
      await api.approveRequest(token, requestIdAction, approverId, actionComment || null);
      setApproverRequests(await api.getApproverRequests(token, approverId));
    });
  }

  async function onReject() {
    await wrap("Reject", async () => {
      await api.rejectRequest(token, requestIdAction, approverId, actionComment || null);
      setApproverRequests(await api.getApproverRequests(token, approverId));
    });
  }

  async function onCancel() {
    await wrap("Cancel", async () => {
      await api.cancelRequest(token, requestIdAction, applicantId, actionComment || null);
      setRequests(await api.getApplicantRequests(token, applicantId));
    });
  }

  async function onLoadBalance() {
    await wrap("Load balance", async () => {
      setBalance(await api.getBalance(token, applicantId));
    });
  }

  async function onLoadDashboard() {
    await wrap("Load dashboard", async () => {
      setDashboard(await api.getDashboard(token, applicantId, 2026, true));
    });
  }

  return (
    <main className="page">
      <section className="hero">
        <h1>ApproveFlow Frontend</h1>
        <p>休暇申請・承認・残数管理・ダッシュボードを操作する Next.js クライアント</p>
        <div className="status">{message}</div>
      </section>

      <section className="card">
        <h2>Auth</h2>
        <form onSubmit={onSaveToken} className="grid">
          <label>JWT Token</label>
          <textarea value={token} onChange={(e) => setToken(e.target.value)} rows={3} placeholder="Bearer token body" />
          <div className="actions">
            <button type="submit">Save Token</button>
            <button type="button" onClick={onLoadToken}>Load Token</button>
          </div>
        </form>
      </section>

      <section className="card">
        <h2>Route Setup</h2>
        <label>Applicant ID</label>
        <input value={applicantId} onChange={(e) => setApplicantId(e.target.value)} />
        <div className="routeList">
          {routeInput.map((step, i) => (
            <div className="routeRow" key={step.stepOrder}>
              <span>Step {step.stepOrder}</span>
              <input value={step.approverId} onChange={(e) => {
                const next = [...routeInput];
                next[i] = { ...next[i], approverId: e.target.value };
                setRouteInput(next);
              }} />
              <input value={step.notificationEmail} onChange={(e) => {
                const next = [...routeInput];
                next[i] = { ...next[i], notificationEmail: e.target.value };
                setRouteInput(next);
              }} />
            </div>
          ))}
        </div>
        <button onClick={onSetRoute}>Save Route</button>
      </section>

      <section className="card">
        <h2>Create Request</h2>
        <form onSubmit={onCreateRequest} className="grid cols2">
          <label>Applicant ID</label>
          <input value={applicantId} onChange={(e) => setApplicantId(e.target.value)} />
          <label>Applicant Name</label>
          <input value={applicantName} onChange={(e) => setApplicantName(e.target.value)} />
          <label>Leave Type</label>
          <input value={leaveType} onChange={(e) => setLeaveType(e.target.value)} />
          <label>Unit Type</label>
          <select value={unitType} onChange={(e) => setUnitType(Number(e.target.value) as 0 | 1)}>
            <option value={0}>Hour</option>
            <option value={1}>Day</option>
          </select>
          <label>Start</label>
          <input type="datetime-local" value={startAt} onChange={(e) => setStartAt(e.target.value)} />
          <label>End</label>
          <input type="datetime-local" value={endAt} onChange={(e) => setEndAt(e.target.value)} />
        </form>
        <label>Reason</label>
        <textarea value={reason} onChange={(e) => setReason(e.target.value)} rows={2} />
        <label className="inline"><input type="checkbox" checked={submitImmediately} onChange={(e) => setSubmitImmediately(e.target.checked)} /> Submit Immediately</label>
        <button onClick={(e) => void onCreateRequest(e as unknown as FormEvent)}>Create</button>
      </section>

      <section className="card">
        <h2>Approval Actions</h2>
        <div className="grid cols2">
          <label>Approver ID</label>
          <input value={approverId} onChange={(e) => setApproverId(e.target.value)} />
          <label>Request ID</label>
          <input value={requestIdAction} onChange={(e) => setRequestIdAction(e.target.value)} />
        </div>
        <label>Comment</label>
        <input value={actionComment} onChange={(e) => setActionComment(e.target.value)} />
        <div className="actions">
          <button onClick={onApprove}>Approve</button>
          <button onClick={onReject}>Reject</button>
          <button onClick={onCancel}>Cancel</button>
        </div>
      </section>

      <section className="card">
        <h2>Requests</h2>
        <div className="actions">
          <button onClick={onLoadApplicant}>Load Applicant</button>
          <button onClick={onLoadApprover}>Load Approver</button>
        </div>
        <div className="tableWrap">
          <table>
            <thead>
              <tr><th>ID</th><th>Applicant</th><th>Status</th><th>Step</th><th>Days</th></tr>
            </thead>
            <tbody>
              {requests.map((r) => (
                <tr key={r.id}><td>{r.id}</td><td>{r.applicantName}</td><td>{statusMap[r.status]}</td><td>{r.currentStepOrder}</td><td>{r.requestedDays}</td></tr>
              ))}
              {approverRequests.map((r) => (
                <tr key={`a-${r.id}`}><td>{r.id}</td><td>{r.applicantName}</td><td>{statusMap[r.status]}</td><td>{r.currentStepOrder}</td><td>{r.requestedDays}</td></tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>

      <section className="card">
        <h2>Balance & Dashboard</h2>
        <div className="actions">
          <button onClick={onLoadBalance}>Load Balance</button>
          <button onClick={onLoadDashboard}>Load Dashboard</button>
        </div>
        {balance && (
          <div className="balance">
            <span>Granted: {balance.grantedDays}</span>
            <span>Used: {balance.usedDays}</span>
            <span>Carry: {balance.carryOverDays}</span>
            <span>Remaining: {balance.remainingDays}</span>
          </div>
        )}
        {dashboard && (
          <div className="bars">
            {dashboard.monthlyStats.map((m) => (
              <div key={m.month} className="barRow">
                <span>{m.month}月</span>
                <div className="bar" style={{ width: `${(m.usedDays / barMax) * 100}%` }} />
                <span>{m.usedDays.toFixed(2)}日</span>
              </div>
            ))}
          </div>
        )}
      </section>
    </main>
  );
}
