import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Layout } from './components/layout/Layout';
import { DashboardPage } from './pages/DashboardPage';
import { CurriculumMatrixPage } from './pages/CurriculumMatrixPage';
import { SyllabusPortfolioPage } from './pages/SyllabusPortfolioPage';
import { MeasurementScoringPage } from './pages/MeasurementScoringPage';
import { ReportsAccreditationPage } from './pages/ReportsAccreditationPage';
import { CqiImprovementPage } from './pages/CqiImprovementPage';
import { IntegrationPortalPage } from './pages/IntegrationPortalPage';
import { GovernanceIamPage } from './pages/GovernanceIamPage';

export const App: React.FC = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<DashboardPage />} />
          <Route path="curriculum-matrix" element={<CurriculumMatrixPage />} />
          <Route path="syllabus-portfolio" element={<SyllabusPortfolioPage />} />
          <Route path="measurement-scoring" element={<MeasurementScoringPage />} />
          <Route path="reports-accreditation" element={<ReportsAccreditationPage />} />
          <Route path="cqi-improvement" element={<CqiImprovementPage />} />
          <Route path="integration-portal" element={<IntegrationPortalPage />} />
          <Route path="governance-iam" element={<GovernanceIamPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default App;
