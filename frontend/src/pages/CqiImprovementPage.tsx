import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Plus,
  X,
  Save,
  CheckCircle,
} from 'lucide-react';
import { EmptyState } from '../components/common/EmptyState';

interface CqiPlanItem {
  id: string;
  code: string;
  title: string;
  ploTarget: string;
  rootCause: string;
  assignedStaff: string;
  deadline: string;
  status: 'DRAFT' | 'APPROVED' | 'IN_PROGRESS' | 'VERIFIED_CLOSED';
  progressPercentage: number;
}

export const CqiImprovementPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/cqi/monitoring')) return 'monitoring';
    return 'action-plans';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  // Clean Dynamic State (Starts with 0 plans)
  const [plans, setPlans] = useState<CqiPlanItem[]>([]);
  const [formTitle, setFormTitle] = useState('');
  const [formPlo, setFormPlo] = useState('PLO5 (Kiểm thử phần mềm)');
  const [formCause, setFormCause] = useState('');

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleCreateCqiPlan = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formTitle.trim()) return;

    setPlans([
      ...plans,
      {
        id: `cqi-${Date.now()}`,
        code: `CQI-${Date.now().toString().slice(-4)}`,
        title: formTitle,
        ploTarget: formPlo,
        rootCause: formCause || 'Chưa ghi nhận',
        assignedStaff: 'Giảng viên phụ trách',
        deadline: '2024-12-31',
        status: 'APPROVED',
        progressPercentage: 0,
      },
    ]);

    setFormTitle('');
    setFormCause('');
    setIsModalOpen(false);
    setToastMessage('✓ Đã tạo thành công Kế hoạch cải tiến CQI mới!');
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
            {activeTab === 'action-plans' && 'Kế Hoạch Cải Tiến Chất Lượng (Bảng Kanban CQI)'}
            {activeTab === 'monitoring' && 'Theo Dõi Tiến Độ & Đo Lường Lại Nghiệm Thu'}
          </h2>
        </div>

        <button onClick={() => setIsModalOpen(true)} className="btn btn-primary">
          <Plus size={16} />
          <span>+ Tạo Kế Hoạch CQI Mới</span>
        </button>
      </div>

      {/* TAB 1: KANBAN BOARD */}
      {activeTab === 'action-plans' && (
        <>
          {plans.length === 0 ? (
            <EmptyState
              title="Chưa có Kế hoạch cải tiến (CQI) nào"
              description="Hiện tại chưa có kế hoạch cải tiến chất lượng nào được mở. Nhấn nút bên dưới để tạo kế hoạch đầu tiên."
              actionLabel="+ Khởi Tạo Kế Hoạch CQI Đầu Tiên"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '1.25rem' }}>
              <div className="glass-card" style={{ padding: '1.25rem' }}>
                <span style={{ fontWeight: 700, fontSize: '0.9rem', color: 'var(--primary-400)' }}>1. Đã Phê Duyệt</span>
                {plans.map((plan) => (
                  <div key={plan.id} style={{ backgroundColor: 'var(--bg-surface-elevated)', padding: '1rem', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)', marginTop: '0.75rem' }}>
                    <span className="badge badge-secondary" style={{ marginBottom: '0.5rem', display: 'inline-block' }}>{plan.code}</span>
                    <h4 style={{ fontSize: '0.9rem', fontWeight: 700, color: 'var(--text-primary)', marginBottom: '0.35rem' }}>{plan.title}</h4>
                    <p style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>• Mục tiêu: {plan.ploTarget}</p>
                  </div>
                ))}
              </div>
            </div>
          )}
        </>
      )}

      {/* TAB 2: MONITORING */}
      {activeTab === 'monitoring' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa có Dữ liệu đo lường lại"
            description="Hiện tại không có kế hoạch cải tiến nào đang trong giai đoạn đo lường lại."
            actionLabel="+ Tạo Kế Hoạch Cải Tiến"
            onAction={() => setIsModalOpen(true)}
          />
        </div>
      )}

      {/* CREATE MODAL */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '500px', maxWidth: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>Khởi Tạo Kế Hoạch Cải Tiến (CQI)</h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleCreateCqiPlan}>
              <div className="form-group">
                <label className="form-label">Tiêu Đề Kế Hoạch</label>
                <input required type="text" placeholder="Nhập tiêu đề..." value={formTitle} onChange={(e) => setFormTitle(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Chuẩn Đầu Ra Mục Tiêu</label>
                <select value={formPlo} onChange={(e) => setFormPlo(e.target.value)} className="form-select">
                  <option>PLO5 (Kiểm thử phần mềm & QA)</option>
                  <option>PLO2 (Phân tích thiết kế hệ thống)</option>
                  <option>PLO4 (Làm việc nhóm & Giao tiếp)</option>
                </select>
              </div>

              <div className="form-group">
                <label className="form-label">Phân Tích Nguyên Nhân Gốc (5-Why)</label>
                <textarea rows={3} placeholder="Ghi nhận nguyên nhân..." value={formCause} onChange={(e) => setFormCause(e.target.value)} className="form-textarea" />
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.5rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Save size={16} /><span>Lưu Kế Hoạch</span></button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
