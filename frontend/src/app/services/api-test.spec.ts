import { TestBed } from '@angular/core/testing';

import { ApiTest } from './api-test';

describe('ApiTest', () => {
  let service: ApiTest;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ApiTest);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
