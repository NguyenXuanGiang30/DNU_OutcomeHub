import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Layout } from './components/layout/Layout';
import { DashboardPage } from './pages/DashboardPage';
import { AcademicDataPage } from './pages/AcademicDataPage';
import { CurriculumMatrixPage } from './pages/CurriculumMatrixPage';
import { SyllabusPortfolioPage } from './pages/SyllabusPortfolioPage';
import { MeasurementScoringPage } from './pages/MeasurementScoringPage';
import { ReportsAccreditationPage } from './pages/ReportsAccreditationPage';
import { CqiImprovementPage } from './pages/CqiImprovementPage';
import { AiAssistantPage } from './pages/AiAssistantPage';
import { GovernanceIamPage } from './pages/GovernanceIamPage';

export const App: React.FC = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          {/* 1. Tổng quan */}
          <Route index element={<DashboardPage />} />

          {/* 2. Dữ liệu đào tạo */}
          <Route path="data/org-units" element={<AcademicDataPage />} />
          <Route path="data/programs" element={<AcademicDataPage />} />
          <Route path="data/cohorts" element={<AcademicDataPage />} />
          <Route path="data/students" element={<AcademicDataPage />} />
          <Route path="data/courses" element={<AcademicDataPage />} />

          {/* 3. Chương trình và chuẩn đầu ra */}
          <Route path="curriculum/programs" element={<CurriculumMatrixPage />} />
          <Route path="curriculum/versions" element={<CurriculumMatrixPage />} />
          <Route path="curriculum/pos" element={<CurriculumMatrixPage />} />
          <Route path="curriculum/plos" element={<CurriculumMatrixPage />} />
          <Route path="curriculum/pis" element={<CurriculumMatrixPage />} />
          <Route path="curriculum/weight-a" element={<CurriculumMatrixPage />} />
          <Route path="curriculum/clos" element={<CurriculumMatrixPage />} />
          <Route path="curriculum/matrix" element={<CurriculumMatrixPage />} />

          {/* 4. Đề cương và đánh giá */}
          <Route path="syllabus/bm13" element={<SyllabusPortfolioPage />} />
          <Route path="syllabus/plans" element={<SyllabusPortfolioPage />} />
          <Route path="syllabus/blueprints" element={<SyllabusPortfolioPage />} />
          <Route path="syllabus/rubrics" element={<SyllabusPortfolioPage />} />
          <Route path="syllabus/approvals" element={<SyllabusPortfolioPage />} />
          <Route path="syllabus/exam-approvals" element={<SyllabusPortfolioPage />} />

          {/* 5. Đo lường chuẩn đầu ra */}
          <Route path="measurement/periods" element={<MeasurementScoringPage />} />
          <Route path="measurement/sources" element={<MeasurementScoringPage />} />
          <Route path="measurement/assignments" element={<MeasurementScoringPage />} />
          <Route path="measurement/sync-grades" element={<MeasurementScoringPage />} />
          <Route path="measurement/rubric-scoring" element={<MeasurementScoringPage />} />
          <Route path="measurement/data-validation" element={<MeasurementScoringPage />} />
          <Route path="measurement/calculation" element={<MeasurementScoringPage />} />
          <Route path="measurement/evidence" element={<MeasurementScoringPage />} />

          {/* 6. Kết quả và cải tiến */}
          <Route path="results/clo" element={<ReportsAccreditationPage />} />
          <Route path="results/pi" element={<ReportsAccreditationPage />} />
          <Route path="results/plo" element={<ReportsAccreditationPage />} />
          <Route path="results/warnings" element={<ReportsAccreditationPage />} />
          <Route path="results/summary-reports" element={<ReportsAccreditationPage />} />
          <Route path="cqi/action-plans" element={<CqiImprovementPage />} />
          <Route path="cqi/monitoring" element={<CqiImprovementPage />} />

          {/* 7. Trợ lý dữ liệu */}
          <Route path="ai/chatbot" element={<AiAssistantPage />} />
          <Route path="ai/analytics" element={<AiAssistantPage />} />
          <Route path="ai/early-warnings" element={<AiAssistantPage />} />

          {/* 8. Quản trị hệ thống */}
          <Route path="governance/users" element={<GovernanceIamPage />} />
          <Route path="governance/roles-scopes" element={<GovernanceIamPage />} />
          <Route path="governance/sis-lms-integration" element={<GovernanceIamPage />} />
          <Route path="governance/audit-logs" element={<GovernanceIamPage />} />
          <Route path="governance/system-config" element={<GovernanceIamPage />} />

          {/* Fallback */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default App;
