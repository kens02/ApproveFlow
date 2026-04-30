export type RequestStatus =
  | "Draft"
  | "Submitted"
  | "InReview"
  | "Approved"
  | "Rejected"
  | "Cancelled";

export interface LeaveRequestPayload {
  applicantId: string;
  applicantName: string;
  leaveType: string;
  unitType: 0 | 1;
  startDateTime: string;
  endDateTime: string;
  reason: string;
  attachmentFileName: string | null;
  submitImmediately: boolean;
}

export interface LeaveRequestSummary {
  id: string;
  applicantId: string;
  applicantName: string;
  leaveType: string;
  unitType: number;
  startDateTime: string;
  endDateTime: string;
  status: number;
  currentStepOrder: number;
  requestedDays: number;
}

export interface RouteStepInput {
  stepOrder: number;
  approverId: string;
  approverName: string;
  notificationEmail: string;
  mailTemplate: string;
}

export interface SetRouteInput {
  applicantId: string;
  steps: RouteStepInput[];
}

export interface ActionInput {
  requestId: string;
  userId: string;
  comment: string | null;
}

export interface LeaveBalanceSummary {
  userId: string;
  grantedDays: number;
  usedDays: number;
  carryOverDays: number;
  remainingDays: number;
}

export interface MonthlyStat {
  month: number;
  usedDays: number;
}

export interface DashboardSummary {
  userId: string;
  remainingDays: number;
  monthlyStats: MonthlyStat[];
}

export interface DevTokenResponse {
  accessToken: string;
  expiresAtUtc: string;
}
