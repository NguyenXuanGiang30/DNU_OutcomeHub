import { apiFetch } from './apiClient';

export interface DashboardMetricSummaryDto {
  totalStudents: number;
  totalActivePeriods: number;
  totalCoursesAssessed: number;
  overallPloAttainmentRate: number;
  pendingCqiPlansCount: number;
  lastDataRefreshTime: string;
}

export interface PloRadarPointDto {
  ploCode: string;
  ploDescription: string;
  attainmentPercentage: number;
  targetThresholdPercentage: number;
  isMet: boolean;
}

export interface EarlyWarningGroupDto {
  groupCode: string;
  name: string;
  atRiskStudentCount: number;
  underperformingPis: string[];
  severity: 'HIGH' | 'MEDIUM' | 'LOW';
}

export interface DashboardResponseDto {
  metrics: DashboardMetricSummaryDto;
  ploRadar: PloRadarPointDto[];
  earlyWarnings: EarlyWarningGroupDto[];
}

export interface AccreditationReportSummaryDto {
  reportId: string;
  standardType: string;
  programName: string;
  overallComplianceScore: number;
  totalPloEvaluated: number;
  cqiCyclesCompleted: number;
  generatedAt: string;
}

export const reportsApi = {
  getDashboardData: (programVersionId?: string, academicYear?: string) =>
    apiFetch<DashboardResponseDto>(
      `/api/v1/dashboard/summary?programVersionId=${programVersionId || ''}&academicYear=${academicYear || ''}`
    ),
  getAccreditationReport: (programVersionId: string, standard: string = 'AUN-QA') =>
    apiFetch<AccreditationReportSummaryDto>(
      `/api/v1/reports/accreditation/${standard}/${programVersionId}`
    ),
};
