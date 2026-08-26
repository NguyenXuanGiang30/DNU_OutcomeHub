import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Download,
  FileText,
  CheckCircle,
} from 'lucide-react';
import { EmptyState } from '../components/common/EmptyState';

export const ReportsAccreditationPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/results/clo')) return 'clo';
    if (location.pathname.includes('/results/pi')) return 'pi';
    if (location.pathname.includes('/results/plo')) return 'plo';
    if (location.pathname.includes('/results/warnings')) return 'warnings';
    return 'summary-reports';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleExport = (type: string) => {
    setToastMessage(`✓ Đang xuất ${type}...`);
    setTimeout(() => setToastMessage(null), 3000);
  };

  return (
    <div className="animate-fade-in">
      {/* Toast Alert */}
      {toastMessage && (
        <div style={{ position: 'fixed', top: '85px', right: '2rem', zIndex: 100, backgroundColor: 'var(--emerald-500)', color: '#fff', padding: '0.75rem 1.25rem', borderRadius: 'var(--radius-md)', boxShadow: 'var(--glass-shadow)', display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: 600 }}>
          <CheckCircle size={18} />
          <span>{toastMessage}</span>
        </div>
      )}

      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <div style={{ fontSize: '0.75rem', color: 'var(--primary-400)', fontWeight: 700, textTransform: 'uppercase', marginBottom: '0.25rem' }}>
            Kết Quả & Cải Tiến
          </div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            {activeTab === 'summary-reports' && 'Báo Cáo Tổng Hợp & Hồ Sơ Tự Đánh Giá'}
            {activeTab === 'plo' && 'Kết Quả Đạt Chuẩn Đầu Ra (PLO)'}
            {activeTab === 'pi' && 'Kết Quả Đạt Từng Chỉ Báo Thực Hiện (PI)'}
            {activeTab === 'clo' && 'Kết Quả Đạt Chuẩn Đầu Ra Học Phần (CLO)'}
            {activeTab === 'warnings' && 'Danh Sách Cảnh Báo Sinh Viên Chưa Đạt Chuẩn'}
          </h2>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button onClick={() => handleExport('Hồ Sơ (.ZIP)')} className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Minh Chứng (.ZIP)</span>
          </button>
          <button onClick={() => handleExport('Báo Cáo (PDF)')} className="btn btn-primary">
            <FileText size={16} />
            <span>Xuất Báo Cáo (PDF)</span>
          </button>
        </div>
      </div>

      {/* TAB: SUMMARY */}
      {activeTab === 'summary-reports' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa có Dữ liệu kết quả đợt đo"
            description="Vui lòng hoàn tất nhập điểm Rubric và chạy tính toán CĐR để xem báo cáo tổng hợp tự đánh giá chuẩn AUN-QA / ABET."
            actionLabel="Đi Tới Màn Hình Tính Toán CĐR"
            onAction={() => navigate('/measurement/calculation')}
          />
        </div>
      )}

      {/* TAB: PLO */}
      {activeTab === 'plo' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa có Kết quả PLO"
            description="Chưa có kết quả tính toán chuẩn đầu ra PLO nào trong hệ thống."
            actionLabel="Đi Tới Chấm Điểm Rubric"
            onAction={() => navigate('/measurement/rubric-scoring')}
          />
        </div>
      )}

      {/* TAB: PI */}
      {activeTab === 'pi' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa có Kết quả Chỉ báo PI"
            description="Chưa có kết quả tính toán chỉ báo thực hiện PI nào."
            actionLabel="Đi Tới Chấm Điểm Rubric"
            onAction={() => navigate('/measurement/rubric-scoring')}
          />
        </div>
      )}

      {/* TAB: CLO */}
      {activeTab === 'clo' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa có Kết quả Môn học CLO"
            description="Chưa có kết quả đạt chuẩn đầu ra học phần nào."
            actionLabel="Đi Tới Chấm Điểm Rubric"
            onAction={() => navigate('/measurement/rubric-scoring')}
          />
        </div>
      )}

      {/* TAB: WARNINGS */}
      {activeTab === 'warnings' && (
        <div className="glass-card">
          <EmptyState
            title="Không có Cảnh báo chưa đạt"
            description="Hệ thống hiện tại không ghi nhận cảnh báo nguy cơ sinh viên chưa đạt nào."
          />
        </div>
      )}
    </div>
  );
};
