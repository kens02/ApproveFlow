import {
  ActionInput,
  DashboardSummary,
  DevTokenResponse,
  LeaveBalanceSummary,
  LeaveRequestPayload,
  LeaveRequestSummary,
  SetRouteInput,
} from "@/lib/types";

const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5000";

function getHeaders(token: string) {
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
}

async function request<T>(path: string, init: RequestInit, fallbackError: string): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, init);
  if (!response.ok) {
    const text = await response.text();
    const body = text || fallbackError;
    if (response.status === 401) {
      throw new Error(`401 Unauthorized: ${body}`);
    }
    if (response.status === 403) {
      throw new Error(`403 Forbidden: ${body}`);
    }
    throw new Error(body);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export const api = {
  getDevToken(userId: string, userName: string, role = "Administrator") {
    return request<DevTokenResponse>("/api/auth/dev-token", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ userId, userName, role }),
    }, "Failed to get dev token");
  },
  createRequest(token: string, payload: LeaveRequestPayload) {
    return request<LeaveRequestSummary>("/api/leave-requests", {
      method: "POST",
      headers: getHeaders(token),
      body: JSON.stringify(payload),
    }, "Failed to create leave request");
  },
  approveRequest(token: string, requestId: string, userId: string, comment: string | null) {
    const payload: ActionInput = { requestId, userId, comment };
    return request<LeaveRequestSummary>(`/api/leave-requests/${requestId}/approve`, {
      method: "POST",
      headers: getHeaders(token),
      body: JSON.stringify(payload),
    }, "Failed to approve");
  },
  rejectRequest(token: string, requestId: string, userId: string, comment: string | null) {
    const payload: ActionInput = { requestId, userId, comment };
    return request<LeaveRequestSummary>(`/api/leave-requests/${requestId}/reject`, {
      method: "POST",
      headers: getHeaders(token),
      body: JSON.stringify(payload),
    }, "Failed to reject");
  },
  cancelRequest(token: string, requestId: string, userId: string, comment: string | null) {
    const payload: ActionInput = { requestId, userId, comment };
    return request<LeaveRequestSummary>(`/api/leave-requests/${requestId}/cancel`, {
      method: "POST",
      headers: getHeaders(token),
      body: JSON.stringify(payload),
    }, "Failed to cancel");
  },
  getApplicantRequests(token: string, applicantId: string) {
    return request<LeaveRequestSummary[]>(`/api/leave-requests/applicant/${encodeURIComponent(applicantId)}`, {
      method: "GET",
      headers: getHeaders(token),
    }, "Failed to fetch applicant requests");
  },
  getApproverRequests(token: string, approverId: string) {
    return request<LeaveRequestSummary[]>(`/api/leave-requests/approver/${encodeURIComponent(approverId)}`, {
      method: "GET",
      headers: getHeaders(token),
    }, "Failed to fetch approver requests");
  },
  setRoute(token: string, payload: SetRouteInput) {
    return request<void>("/api/routes", {
      method: "PUT",
      headers: getHeaders(token),
      body: JSON.stringify(payload),
    }, "Failed to set route");
  },
  getBalance(token: string, userId: string) {
    return request<LeaveBalanceSummary>(`/api/balances/${encodeURIComponent(userId)}`, {
      method: "GET",
      headers: getHeaders(token),
    }, "Failed to fetch balance");
  },
  getDashboard(token: string, userId: string, year: number, fiscalYear: boolean) {
    return request<DashboardSummary>(`/api/dashboard/${encodeURIComponent(userId)}?year=${year}&fiscalYear=${fiscalYear}`, {
      method: "GET",
      headers: getHeaders(token),
    }, "Failed to fetch dashboard");
  },
};
