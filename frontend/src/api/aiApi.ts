import { apiFetch } from './apiClient';

export interface AiCitationDto {
  sourceType: string;
  sourceIdentifier: string;
  title: string;
  pageOrSection?: string;
  dataTimestamp: string;
  formulaApplied?: string;
}

export interface AiChatResponseDto {
  answer: string;
  citations: AiCitationDto[];
  containsMaskedPersonalData: boolean;
  totalSourcesRetrieved: number;
  confidenceScore: number;
  modelUsed: string;
  generatedAt: string;
}

export interface AiAnomalyIssueDto {
  issueCode: string;
  severity: 'CRITICAL' | 'WARNING' | 'INFO';
  category: string;
  description: string;
  affectedEntity: string;
  suggestedRemediation: string;
}

export interface AiAnomalyDetectionResultDto {
  programVersionId: string;
  programCode: string;
  totalIssuesFound: number;
  criticalCount: number;
  warningCount: number;
  issues: AiAnomalyIssueDto[];
  diagnosedAt: string;
}

export interface AiExtractedFieldDto {
  fieldName: string;
  extractedValue: string;
  sourcePageNumber: number;
  confidence: number;
  isInferred: boolean;
}

export interface AiExtractionResultDto {
  extractionId: string;
  documentId: string;
  documentType: string;
  extractedFields: AiExtractedFieldDto[];
  overallConfidence: number;
  status: string;
  extractedAt: string;
}

export const aiApi = {
  queryChatbot: (prompt: string, sessionId: string = 'SESSION_WEB_01') =>
    apiFetch<AiChatResponseDto>('/api/v1/ai/chat/query', {
      method: 'POST',
      body: JSON.stringify({ prompt, conversationSessionId: sessionId }),
    }),
  runDiagnostics: (programVersionId: string) =>
    apiFetch<AiAnomalyDetectionResultDto>(`/api/v1/ai/diagnostics/curriculum/${programVersionId}`),
  extractDocument: (documentType: string, filePath: string) =>
    apiFetch<AiExtractionResultDto>('/api/v1/ai/extract/document', {
      method: 'POST',
      body: JSON.stringify({
        documentId: '00000000-0000-0000-0000-000000000001',
        documentType,
        filePath,
        targetSchemaVersion: 'v1.0',
      }),
    }),
};
