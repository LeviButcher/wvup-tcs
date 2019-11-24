import React from 'react';
import ScaleLoader from 'react-spinners/ScaleLoader';
import StartToEndDateForm from '../../components/StartToEndDateForm';
import ClassToursTable from '../../components/ClassToursTable';
import { Paging, Link, Button, Card } from '../../ui';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

const take = 20;
const getClassTourUrl = (start, end, page = 1) => {
  if (start == null || end == null) return '';
  const endpoint = `classtours/?start=${start}&end=${end}&skip=${page * take -
    take}&take=${take}`;
  return endpoint;
};

type Props = {
  navigate: string => any,
  startDate?: string,
  endDate?: string,
  page?: number
};

const ClassTourLookup = ({
  navigate,
  startDate = '',
  endDate = '',
  page = 1
}: Props) => {
  const endpoint = getClassTourUrl(startDate, endDate, page);
  const [loading, data, errors] = useApiWithHeaders(endpoint);

  return (
    <div>
      <Card>
        <h3>Additional Actions</h3>
        <div
          style={{
            display: 'grid',
            justifyContent: 'space-between',
            gridTemplateColumns: 'auto auto',
            gridGap: '1rem'
          }}
        >
          <Link to="create">
            <Button intent="primary" align="left">
              Add Class Tour
            </Button>
          </Link>
        </div>
      </Card>
      <StartToEndDateForm
        title="ClassTour Lookup"
        onSubmit={values => {
          return Promise.resolve(
            navigate(`/dashboard/tours/${values.startDate}/${values.endDate}`)
          );
        }}
        initialValues={{
          startDate,
          endDate
        }}
      />
      <ScaleLoader loading={loading} />
      {!loading && (
        <>
          {data.body.length <= 0 ? (
            <div>No records found</div>
          ) : (
            <Card width="auto">
              <Paging
                currentPage={data.headers['current-page']}
                totalPages={data.headers['total-pages']}
                next={data.headers.next}
                prev={data.headers.prev}
                queries={{}}
                baseURL={`/dashboard/tours/${startDate}/${endDate}/`}
              />
              <ClassToursTable classTours={data.body} />
              <Paging
                currentPage={data.headers['current-page']}
                totalPages={data.headers['total-pages']}
                next={data.headers.next}
                prev={data.headers.prev}
                queries={{}}
                baseURL={`/dashboard/tours/${startDate}/${endDate}/`}
              />
            </Card>
          )}
        </>
      )}
      {errors && errors.message && <div>{errors.message}</div>}
    </div>
  );
};

ClassTourLookup.defaultProps = {
  page: 1,
  startDate: '',
  endDate: ''
};

export default ClassTourLookup;
