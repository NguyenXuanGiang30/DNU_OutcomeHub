import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Download,
  Plus,
  X,
  Save,
  CheckCircle,
} from 'lucide-react';
import { EmptyState } from '../components/common/EmptyState';

interface AssessmentPlanItem {
  code: string;
  name: string;
  method: string;
  weight: number;
  clos: string;
  role: string;
}

export const SyllabusPortfolioPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/syllabus/plans')) return 'plans';
    if (location.pathname.includes('/syllabus/blueprints')) return 'blueprints';
    if (location.pathname.includes('/syllabus/rubrics')) return 'rubrics';
    if (location.pathname.includes('/syllabus/approvals')) return 'approvals';
    if (location.pathname.includes('/syllabus/exam-approvals')) return 'exam-approvals';
    return 'bm13';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  // Clean Dynamic States
  const [plans, setPlans] = useState<AssessmentPlanItem[]>([]);
  const [formPlanName, setFormPlanName] = useState('');
  const [formPlanWeight, setFormPlanWeight] = useState(30);

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleSaveModal = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formPlanName.trim()) return;

    setPlans([
      ...plans,
      {
        code: `A${plans.length + 1}`,
        name: formPlanName,
        method: 'Thực hành máy tính',
        weight: formPlanWeight,
        clos: 'CLO1, CLO2',
        role: 'Đo Trực Tiếp (A)',
      },
    ]);

    setFormPlanName('');
    setIsModalOpen(false);
    setToastMessage('✓ Đã lưu thành công bài đánh giá mới!');
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
            Đề Cương & Đánh Giá
          </div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            {activeTab === 'bm13' && 'Đề Cương Chi Tiết Học Phần (Mẫu BM13)'}
            {activeTab === 'plans' && 'Kế Hoạch Đánh Giá Học Phần (A1, A2, A3)'}
            {activeTab === 'blueprints' && 'Ma Trận Đề Thi & Ngân Hàng Câu Hỏi'}
            {activeTab === 'rubrics' && 'Tiêu Chí Chấm Điểm Rubric Định Lượng'}
            {activeTab === 'approvals' && 'Quy Trình Ký Duyệt Số Hóa Đề Cương BM13'}
            {activeTab === 'exam-approvals' && 'Phê Duyệt Đề Thi & Đáp Án Thang Điểm'}
          </h2>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button onClick={() => setIsModalOpen(true)} className="btn btn-primary">
            <Plus size={16} />
            <span>+ Thêm Mới / Cập Nhật</span>
          </button>
          <button className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Hồ Sơ</span>
          </button>
        </div>
      </div>

      {/* TAB 2: PLANS */}
      {activeTab === 'plans' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Kế Hoạch Đánh Giá Học Phần (A1, A2, A3)</h3>
              <p className="glass-card-subtitle">Phân bổ tỷ trọng điểm môn học theo quy định khảo thí (Tổng 100%)</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Bài Đánh Giá</button>
          </div>

          {plans.length === 0 ? (
            <EmptyState
              title="Chưa có Kế hoạch đánh giá nào"
              description="Hiện tại học phần này chưa có bài đánh giá (A1, A2, A3). Nhấn nút bên dưới để thêm bài đánh giá mới."
              actionLabel="+ Thêm Bài Đánh Giá Đầu Tiên"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Mã</th>
                    <th>Tên Bài Đánh Giá</th>
                    <th>Hình Thức</th>
                    <th>Tỷ Trọng (%)</th>
                    <th>CLO Đánh Giá</th>
                    <th>Vai Trò</th>
                  </tr>
                </thead>
                <tbody>
                  {plans.map((p) => (
                    <tr key={p.code}>
                      <td><strong className="badge badge-primary">{p.code}</strong></td>
                      <td style={{ fontWeight: 600 }}>{p.name}</td>
                      <td>{p.method}</td>
                      <td><strong>{p.weight}%</strong></td>
                      <td>{p.clos}</td>
                      <td><span className="badge badge-danger">{p.role}</span></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* TAB 1: BM13 */}
      {activeTab === 'bm13' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa chọn Đề cương chi tiết học phần"
            description="Vui lòng chọn học phần từ danh mục để xem hoặc biên soạn đề cương BM13."
            actionLabel="+ Biên Soạn Đề Cương Mới"
            onAction={() => setIsModalOpen(true)}
          />
        </div>
      )}

      {/* TAB: BLUEPRINTS */}
      {activeTab === 'blueprints' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa có Ma trận đề thi (Blueprint)"
            description="Hiện tại chưa có cấu trúc đề thi nào được tạo cho học phần này."
            actionLabel="+ Tạo Cấu Trúc Đề Thi"
            onAction={() => setIsModalOpen(true)}
          />
        </div>
      )}

      {/* TAB: RUBRICS */}
      {activeTab === 'rubrics' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa có Tiêu chí Rubric"
            description="Hiện tại chưa có tiêu chí chấm Rubric nào được định nghĩa."
            actionLabel="+ Thêm Tiêu Chí Rubric"
            onAction={() => setIsModalOpen(true)}
          />
        </div>
      )}

      {/* TAB: APPROVALS */}
      {activeTab === 'approvals' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa có Yêu cầu phê duyệt đề cương"
            description="Hiện tại không có hồ sơ đề cương nào đang chờ duyệt."
            actionLabel="+ Nộp Đề Cương Phê Duyệt"
            onAction={() => setIsModalOpen(true)}
          />
        </div>
      )}

      {/* TAB: EXAM APPROVALS */}
      {activeTab === 'exam-approvals' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa có Đề thi chờ phê duyệt"
            description="Hiện tại không có đề thi nào đang chờ hội đồng khảo thí ký duyệt."
            actionLabel="+ Nộp Đề Thi Phê Duyệt"
            onAction={() => setIsModalOpen(true)}
          />
        </div>
      )}

      {/* MODAL DIALOG */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '500px', maxWidth: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>Thêm Bài Đánh Giá Mới</h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleSaveModal}>
              <div className="form-group">
                <label className="form-label">Tên Bài Đánh Giá</label>
                <input required type="text" placeholder="Ví dụ: Bài thực hành kiểm thử..." value={formPlanName} onChange={(e) => setFormPlanName(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Tỷ Trọng Điểm (%)</label>
                <input required type="number" step="5" min="5" max="100" value={formPlanWeight} onChange={(e) => setFormPlanWeight(Number(e.target.value))} className="form-input" />
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.5rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Save size={16} /><span>Lưu Thay Đổi</span></button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
