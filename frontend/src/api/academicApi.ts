import { apiFetch } from './apiClient';

export interface OrgUnitDto {
  id: string;
  code: string;
  name: string;
  type: string;
}

export interface ProgramPloDto {
  id: string;
  code: string;
  description: string;
  bloomLevel: string;
  isLocked: boolean;
}

export interface ProgramVersionDto {
  id: string;
  code: string;
  versionNo: number;
  status: string;
  totalCredits: number;
  plos: ProgramPloDto[];
}

export interface StudentPathCoverageDto {
  programVersionId: string;
  studentPathCode: string;
  isFullyCovered: boolean;
  totalPlosRequired: number;
  totalPlosCovered: number;
  coveragePercentage: number;
  uncoveredPis: string[];
}

export interface PrerequisiteEdgeDto {
  sourceCourseCode: string;
  sourceCourseName: string;
  targetCourseCode: string;
  targetCourseName: string;
  type: string;
}

export interface PrerequisiteGraphDto {
  programVersionId: string;
  totalNodes: number;
  totalEdges: number;
  edges: PrerequisiteEdgeDto[];
  criticalPath: string[];
}

export const academicApi = {
  getOrgUnits: () => apiFetch<OrgUnitDto[]>('/api/v1/academic/org-units'),
  getProgramPlos: (programVersionId: string) =>
    apiFetch<ProgramPloDto[]>(`/api/v1/academic/programs/versions/${programVersionId}/plos`),
  getStudentPathCoverage: (programVersionId: string, pathCode: string = 'PATH_MAIN') =>
    apiFetch<StudentPathCoverageDto>(`/api/v1/academic/matrix/coverage/${programVersionId}/${pathCode}`),
  getPrerequisitesGraph: (programVersionId: string) =>
    apiFetch<PrerequisiteGraphDto>(`/api/v1/academic/matrix/prerequisites/${programVersionId}`),
};
